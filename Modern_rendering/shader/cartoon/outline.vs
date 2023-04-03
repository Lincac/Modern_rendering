#version 460 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform float outlineRange;

void main(){
    vec4 worldPos = view * model * vec4(aPos,1.0);
    mat3 normalMatrix = transpose(inverse(mat3(view * model)));
    vec3 Normal = normalMatrix * aNormal;
    Normal.z = -0.5;
    
    worldPos = worldPos + vec4(normalize(Normal),0) * outlineRange;

    gl_Position = projection * worldPos;
}