#version 330 core
out vec4 FragColor;

in VS_OUT{
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoord;
}fs_in;

uniform sampler2D BOX;

void main()
{    
    // FragColor = texture(BOX, fs_in.TexCoord);
    FragColor = vec4(0,1,0,1);
}