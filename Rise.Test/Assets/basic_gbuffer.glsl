#version 330
uniform mat4 g_Matrix;
layout(location = 0) in vec2 v_Pos;
layout(location = 1) in vec2 v_Tex;
out vec2 f_Tex;
void main(void)
{
    gl_Position = g_Matrix * vec4(v_Pos, 0.0, 1.0);
    f_Tex = v_Tex;
}
...
#version 330
uniform sampler2D g_Diffuse;
uniform sampler2D g_Normal;
uniform sampler2D g_Position;
in vec2 f_Tex;
layout(location = 0) out vec4 o_Col;
void main(void)
{
    o_Col = texture(g_Diffuse, f_Tex);
}
