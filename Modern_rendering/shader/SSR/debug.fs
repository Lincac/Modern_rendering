#version 450 core

out vec4 FragColor;

in vec2 TexCoords;

float near = 0.1; 
float far  = 1000.0; 

float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));    
}

uniform sampler2D ssrcolor;
uniform sampler2D Albedo;

void main(){
    // float depth = LinearizeDepth(textureLod(ssrcolor,TexCoords,0).r) / far; // 为了演示除以 far
    // float depth = textureLod(depthMap,TexCoords,1).r;
    // FragColor = vec4(vec3(depth), 1.0);
    // FragColor = texture(depthMap,TexCoords);

    // vec2 uv = texture(depthMap,TexCoords).xy;
    // vec3 color = texture(Albedo,uv).rgb;
    FragColor = texture(ssrcolor,TexCoords) + texture(Albedo,TexCoords);
    // FragColor = texture(ssrcolor,TexCoords);
}