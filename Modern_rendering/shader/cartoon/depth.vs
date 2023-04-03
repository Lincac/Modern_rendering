#version 460 core
layout(location=0) in vec3 aPos;
layout(location=1) in vec3 aNormal;
layout(location=2) in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 light_VP;

void main(){
    gl_Position = light_VP * model * vec4(aPos,1.0);
}