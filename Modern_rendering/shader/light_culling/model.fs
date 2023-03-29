#version 460 core

out vec4 FragColor;

in VS_OUT{
    vec3 WorldPos;
    vec3 WorldNormal;
    vec2 TexCoords;
}fs_in;

struct Light{
    vec4 position;
    vec4 color;
};

struct VisalbeIndex{
    int index;
};

layout(std430, binding = 0) readonly buffer LightBuffer  {
    Light data[];
}lightBuffer;

layout(std430, binding = 1) readonly buffer VisalbeIndexBuffer  {
    VisalbeIndex data[];
}visalbeindexBuffer;

uniform sampler2D TextureMap;
uniform vec3 camPos;
uniform int NumOfTilex;

// 计算点光源的衰弱
float attenuate(vec3 lightDirection, float radius) {
	float cutoff = 0.5;
	float attenuation = dot(lightDirection, lightDirection) / (100.0 * radius);
	attenuation = 1.0 / (attenuation * 15.0 + 1.0);
	attenuation = (attenuation - cutoff) / (1.0 - cutoff);
 
	return clamp(attenuation, 0.0, 1.0);
}

void main(){
    // gl_FragCoord 代表当前屏幕的坐标
	ivec2 location = ivec2(gl_FragCoord.xy);
	ivec2 tileID = location / ivec2(16, 16);
	uint index = tileID.y * NumOfTilex + tileID.x; // 获得当前像素所在Tile索引 

    vec3 WorldPos = fs_in.WorldPos;
    vec3 WorldNormal = normalize(fs_in.WorldNormal);
    vec3 viewDir = normalize(camPos - fs_in.WorldPos);

    vec3 albedo = pow(texture(TextureMap,fs_in.TexCoords).rgb,vec3(2.2));
    vec3 ambient = vec3(0.08) * albedo;

    vec3 color = vec3(0);

    uint offset = index * 256;
    for(int i=0;i<256 && visalbeindexBuffer.data[offset + i].index != -1 ;i++){
        uint lightindex = visalbeindexBuffer.data[offset + i].index;
        Light light = lightBuffer.data[lightindex];

        vec3 lightPos = light.position.xyz;
        vec3 lightCol = light.color.xyz;
        float radius = light.position.w;

        // 光源衰减
        vec3 lightDir = lightPos - fs_in.WorldPos;
        float atten = attenuate(lightDir,radius);

        lightDir = normalize(lightDir);
        vec3 halfDir = normalize(viewDir + lightDir);

        float diff = max(dot(lightDir, WorldNormal), 0.0);
        float spec = pow(max(dot(WorldNormal, halfDir), 0.0), 32.0);    

        if(diff == 0.0){
            spec = 0.0; // 不计算阴影
        }

        vec3 irradiance = lightCol * ((albedo * diff) + vec3(spec)) * atten;
        color += irradiance;
    }
    color += ambient;

    FragColor = vec4(color,1.0);
}