#version 330 core
layout (location = 0) out vec4 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedo;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

uniform float roughness;

void main()
{    
    // store the fragment position vector in the first gbuffer texture
    gPosition.xyz = FragPos;
    gPosition.a = gl_FragCoord.z;
    // also store the per-fragment normals into the gbuffer
    gNormal = normalize(Normal);
    // and the diffuse per-fragment color
    gAlbedo.rgb = vec3(0.95);
    gAlbedo.a = roughness;
}