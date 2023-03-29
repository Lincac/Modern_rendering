#version 450 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out VS_OUT{
    vec3 WordPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 curpos;
    vec4 hispos;
}vs_out;

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform float SCR_WIDTH;
uniform float SCR_HEIGHT;
uniform int offsetid;

const vec2 Halton_2_3[8] =
{
    vec2(0.0f, -1.0f / 3.0f),
    vec2(-1.0f / 2.0f, 1.0f / 3.0f),
    vec2(1.0f / 2.0f, -7.0f / 9.0f),
    vec2(-3.0f / 4.0f, -1.0f / 9.0f),
    vec2(1.0f / 4.0f, 5.0f / 9.0f),
    vec2(-1.0f / 4.0f, -5.0f / 9.0f),
    vec2(3.0f / 4.0f, 1.0f / 9.0f),
    vec2(-7.0f / 8.0f, 7.0f / 9.0f)
};

layout (std140,binding = 0) uniform preMat
{
    mat4 premodel;
    mat4 preview;
    mat4 preprojection;
};

void main()
{
    vs_out.TexCoords = aTexCoords;
    vs_out.WordPos = vec3(model * vec4(aPos,1.0));
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vs_out.Normal = normalMatrix * aNormal;

    vs_out.curpos = projection * view * model * vec4(aPos,1.0);
    vs_out.hispos = preprojection * preview * premodel * vec4(aPos,1.0);

    vec2 texSize = vec2(1.0 / SCR_WIDTH,1.0 / SCR_HEIGHT);
    vec2 offset = Halton_2_3[offsetid] * texSize;
    mat4 temp = projection;
    temp[2][0] += offset.x;
    temp[2][1] += offset.y;

    gl_Position = temp * view * model * vec4(aPos,1.0); // 所谓的抖动其实就是渲染点的偏移
}