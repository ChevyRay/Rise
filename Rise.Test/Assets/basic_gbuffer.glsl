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
uniform sampler2D ColorMap;
uniform sampler2D NormalMap;
uniform sampler2D PositionMap;
in vec2 f_Tex;
layout(location = 0) out vec4 o_Col;
void main(void)
{
    o_Col = texture(g_Diffuse, f_Tex);
}
