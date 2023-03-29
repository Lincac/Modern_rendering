#version 460 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D depthMap;
uniform float near_plane;
uniform float far_plane;
uniform int layer;

// required when using a perspective projection matrix
float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // Back to NDC 
    return (2.0 * near_plane * far_plane) / (far_plane + near_plane - z * (far_plane - near_plane));	
}

void main()
{             
    float depthValue = texture(depthMap,TexCoords).r;
    FragColor = vec4(vec3(LinearizeDepth(depthValue) / far_plane), 1.0); // perspective
}

// uniform sampler2D gPosition;

// void main(){
//     float red = texture(gPosition,TexCoords).r;
//     FragColor = vec4(red,red,red,1.0);
//     // FragColor = vec4(texture(gPosition,TexCoords).xyz,1.0);
//     // texture(gPosition,TexCoords).xyz 有区别
// }