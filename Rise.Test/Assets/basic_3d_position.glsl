#version 330
uniform mat4 Matrix;
layout(location = 0) in vec2 vertPos;
layout(location = 1) in vec2 vertUV;
out vec2 fragUV;
void main(void)
{
    gl_Position = Matrix * vec4(vertPos, 0.0, 1.0);
    fragUV = vertUV;
}
...
#version 330
uniform sampler2D ZBuffer;
uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
in vec2 fragUV;
layout(location = 0) out vec4 outColor;
void main(void)
{
    float z = texture(ZBuffer, fragUV).r * 2.0 - 1.0;
    vec4 clipSpacePosition = vec4(fragUV * 2.0 - 1.0, z, 1.0);
    vec4 viewSpacePosition = inverse(ProjMatrix) * clipSpacePosition;
    viewSpacePosition /= viewSpacePosition.w;
    vec4 worldSpacePosition = inverse(ViewMatrix) * viewSpacePosition;
    outColor = vec4(worldSpacePosition.xyz, 1.0);
}
