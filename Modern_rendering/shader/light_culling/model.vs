#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out VS_OUT{
    vec3 WorldPos;
    vec3 WorldNormal;
    vec2 TexCoords;
}vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
    vs_out.WorldPos = vec3(model * vec4(aPos,1.0));
    mat3 normalMatrix = transpose(inverse(mat3(view * model)));
    vs_out.WorldNormal = normalMatrix * aNormal;
    vs_out.TexCoords = aTexCoords;

    gl_Position = projection * view * vec4(vs_out.WorldPos,1.0);
}
