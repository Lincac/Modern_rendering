#version 450 core

in VS_OUT{
    vec3 WordPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 curpos;
    vec4 hispos;
}fs_in;

layout (location = 0) out vec4 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAlbedo;
layout (location = 3) out vec2 gVelo;

uniform sampler2D texture_diffuse1; 

const float near = 0.1f;
const float far = 100.0f;

float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));    
}

void main()
{    
    gPosition.xyz = fs_in.WordPos;
    gPosition.w = LinearizeDepth(gl_FragCoord.z) / far;
    gNormal.xyz = normalize(fs_in.Normal);
    gAlbedo = texture(texture_diffuse1,fs_in.TexCoords);

    vec2 cur = (fs_in.curpos.xy / fs_in.curpos.w) * 0.5 + 0.5;
    vec2 his = (fs_in.hispos.xy / fs_in.hispos.w) * 0.5 + 0.5;
    gVelo = cur - his; 
}