#version 460 core

out vec4 FragColor;
in vec2 TexCoords;

uniform sampler2D TexMap;

void main(){
    FragColor = texture(TexMap,TexCoords);
}