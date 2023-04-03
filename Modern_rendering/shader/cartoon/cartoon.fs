#version 460 core

out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform sampler2D TexMap;
uniform sampler2D Ramp;
uniform sampler2D depthMap;
uniform vec3 ColorTint;
uniform float specularScale;
uniform vec3 lightPos;
uniform vec3 lightCol;
uniform vec3 viewPos;

uniform mat4 view;
uniform mat4 light_VP;

float lerp(float x,float y,float weight){
    return x + (y - x) * weight;
}

float RadicalInverse_VdC(uint bits) // (0,1) 低差异序列
{
     bits = (bits << 16u) | (bits >> 16u);
     bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
     bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
     bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
     bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
     return float(bits) * 2.3283064365386963e-10; // / 0x100000000
}

float get_average_field(vec3 currentPos,vec2 texelSize){
    int step = 3;
    float over_depth = 0.0;
    float sum_depth = 0.0;
    for(int x = -step ; x < step ;++x){
        for(int y = -step ; y < step ;++y){
            float sampleDepth = texture(depthMap, vec2(currentPos.xy + vec2(x,y) * texelSize)).r;
            if(sampleDepth < currentPos.z){
                over_depth += sampleDepth;
                sum_depth += 1;
            }
        }
    }
    if(over_depth > 0.0) return over_depth / sum_depth;
    return 0.0;
}

float ShadowCalculation(vec3 fragPosWorldSpace)
{
    vec4 fragPosLightSpace = light_VP * vec4(fragPosWorldSpace,1.0);
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;

    float current = projCoords.z;
    if(current > 1.0) return 0.0;

    float bias = max(0.05 * (1.0 - dot(normalize(Normal),normalize(lightPos))),0.005);

    // pcss
    vec2 texelSize = 1.0 / vec2(textureSize(depthMap,0));
    float avg_depth = get_average_field(projCoords,texelSize);  // 滤核小于目前点的平均深度
    if(avg_depth == 0.0) return 0.0;
    float d_blocker = avg_depth;
    float d_receiver = current;
    float filt = (d_receiver - d_blocker) * 1000 * 2 / d_blocker;
    if(filt == 0) return 0.0;

    // pcf
    filt = filt < 1.0 ? 2.0 : filt;
    float shadow = 0.0;
    int cout = 0;
    for(uint x = 0; x <= 4; x++)
    {
        cout++;
        for(uint y = 0; y <= 4; y++)
        {
            float Hx = RadicalInverse_VdC(x);
            float Hy = RadicalInverse_VdC(y);
            if(x / 2 == 0) Hx *= -1;
            if(y / 2 == 0) Hy *= -1;
            float pcfDepth = texture(depthMap, vec2(projCoords.xy + vec2(Hx * filt,Hy * filt) * texelSize)).r;
            shadow += (current - bias) > pcfDepth ? 1.0 : 0.0;      
        }    
    }
    shadow /= cout * cout;
    if(projCoords.z>1.0) shadow = 0.0;
    
    return shadow; 
}

void main(){
    vec3 worldNormal = normalize(Normal);
    vec3 lightDir = normalize(lightPos);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 halfDir = normalize(viewDir + lightDir);

    vec4 color = texture(TexMap,TexCoords);
    vec3 albedo = vec3(1) * ColorTint;
    vec3 ambient = vec3(0.03) * albedo;

    float atten = ShadowCalculation(FragPos);

    float diff = dot(worldNormal,lightDir);
    diff = (diff * 0.5 + 0.5) * atten;

    vec3 diffuse = lightCol * albedo * texture(Ramp,vec2(diff,diff)).rgb;

    float spec = dot(worldNormal,halfDir);
    float w = fwidth(spec) * 2.0;
    vec3 specular = vec3(1) * lerp(0,1,smoothstep(-w,w,spec + specularScale - 1)) * step(0.0001,specularScale);

    FragColor = vec4(ambient + diffuse + specular,1.0);
}