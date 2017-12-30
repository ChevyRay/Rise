#version 330
uniform mat4 Matrix;
layout(location = 0) in vec2 vertPos;
layout(location = 1) in vec2 vertUV;
layout(location = 2) in vec4 vertMult;
layout(location = 3) in vec4 vertAdd;
out vec2 fragUV;
out vec4 fragMult;
out vec4 fragAdd;
void main(void)
{
    gl_Position = Matrix * vec4(vertPos, 0.0, 1.0);
    fragUV = vertUV;
    fragMult = vertMult;
    fragAdd = vertAdd;
}
...
#version 330
uniform sampler2D Texture;
in vec2 fragUV;
in vec4 fragMult;
in vec4 fragAdd;
layout(location = 0) out vec4 outColor;
void main(void)
{
    vec4 color = texture(Texture, fragUV);
    outColor = color * fragMult + fragAdd * color.a;
}
