#version 460 core
out vec4 FragColor;

in vec2 TexCoords;

struct Light{
    vec4 position;
    vec4 color;
};

struct VisalbeIndex{
    int index;
};

layout(std430, binding = 0) readonly buffer LightBuffer  {
    Light data[];
}lightBuffer;

layout(std430, binding = 1) readonly buffer VisalbeIndexBuffer  {
    VisalbeIndex data[];
}visalbeindexBuffer;

uniform sampler2D tex;
uniform mat4 projection;
uniform int NumOfTilex;
uniform int time;

const float near_plane = 0.1f;
const float far_plane = 500.0f;
float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // Back to NDC 
    return (2.0 * near_plane * far_plane) / (far_plane + near_plane - z * (far_plane - near_plane)) / far_plane;	
}

void main()
{    
    ivec2 location = ivec2(gl_FragCoord.xy);
	ivec2 tileID = location / ivec2(16, 16);
	uint Tileindex = tileID.y * NumOfTilex + tileID.x; // 获得当前像素所在Tile索引 

    float depth = texture(tex, TexCoords).r;
    depth = (0.5 * projection[3][2]) / (depth + 0.5 * projection[2][2] - 0.5) / far_plane;

    uint offset = Tileindex * 256;
    VisalbeIndex visableindex = visalbeindexBuffer.data[offset + time];
    Light light = lightBuffer.data[visableindex.index];

    FragColor = vec4(light.color.xyz,1.0);
}