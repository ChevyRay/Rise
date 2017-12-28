#version 330
uniform mat4 g_ModelViewProjection;
uniform mat4 g_Model;
layout(location=0) in vec3 v_Pos;
layout(location=1) in vec3 v_Nor;
layout(location=2) in vec2 v_Tex;
layout(location=3) in vec4 v_Col;
out vec4 f_Pos;
out vec3 f_Nor;
out vec2 f_Tex;
out vec3 f_Col;
void main()
{
	vec4 pos = vec4(v_Pos, 1.0);
    gl_Position = g_ModelViewProjection * pos;
	f_Pos = g_Model * pos;
    f_Nor = transpose(inverse(mat3(g_Model))) * v_Nor;
    f_Tex = v_Tex;
    f_Col = v_Col.rgb;
}
...
#version 330
uniform sampler2D g_Texture;
in vec4 f_Pos;
in vec3 f_Nor;
in vec2 f_Tex;
in vec3 f_Col;
layout(location = 0) out vec3 o_Dif;
layout(location = 1) out vec3 o_Nor;
layout(location = 2) out vec3 o_Pos;
void main(void)
{
    o_Dif = texture(g_Texture, f_Tex).rgb * f_Col;
    o_Nor = f_Nor;
    o_Pos = f_Pos.xyz;
}
