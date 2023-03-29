#version 450 core

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedo;
layout (location = 3) out vec4 gParameter;

uniform sampler2D Albedo;

uniform float index;

in vec3 fragpos;
in vec2 TexCoords;
in vec3 Normal;

void main(){
    gPosition = fragpos;
    gNormal = normalize(Normal);
    gAlbedo = texture(Albedo,TexCoords);
    gParameter = vec4(index);
}