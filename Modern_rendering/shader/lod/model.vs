#version 460 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;
layout (location = 5) in vec4 am_BoneIDs;
layout (location = 6) in vec4 am_Weights;

out VS_OUT{
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
    vec3 Tangent;
    vec3 Bitangent;
    vec4 m_BoneIDs;
    vec4 m_Weights;
}vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vs_out.FragPos = vec3(model * vec4(aPos,1.0));
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vs_out.Normal = normalMatrix * aNormal;
    vs_out.TexCoords = aTexCoords;
    vs_out.Tangent = aTangent;
    vs_out.Bitangent = aBitangent;
    vs_out.m_BoneIDs = am_BoneIDs;
    vs_out.m_Weights = am_Weights;
    
    gl_Position = projection * view * vec4(vs_out.FragPos,1.0);
}