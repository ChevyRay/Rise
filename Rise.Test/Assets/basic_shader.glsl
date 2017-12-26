#version 330
uniform mat4 g_Matrix;
layout(location = 0) in vec3 v_Pos;
layout(location = 1) in vec2 v_Tex;
layout(location = 2) in vec4 v_Mul;
layout(location = 3) in vec4 v_Add;
out vec2 f_Tex;
out vec4 f_Mul;
out vec4 f_Add;
void main(void)
{
    gl_Position = g_Matrix * vec4(v_Pos, 1.0);
    f_Tex = v_Tex;
    f_Mul = v_Mul;
    f_Add = v_Add;
}
...
#version 330
uniform sampler2D g_Texture;
in vec2 f_Tex;
in vec4 f_Mul;
in vec4 f_Add;
layout(location = 0) out vec4 o_Col;
void main(void)
{
    o_Col = texture(g_Texture, f_Tex) * f_Mul + f_Add;
}
