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
out vec4 f_Col;
void main()
{
	vec4 pos = vec4(v_Pos, 1.0);
    gl_Position = g_ModelViewProjection * pos;
	f_Pos = g_Model * pos;
    f_Nor = transpose(inverse(mat3(g_Model))) * v_Nor;
    f_Tex = v_Tex;
    f_Col = v_Col;
}
...
#version 330
uniform sampler2D g_Texture;
uniform vec4 g_AmbientColor;
in vec4 f_Pos;
in vec3 f_Nor;
in vec2 f_Tex;
in vec4 f_Col;
out vec4 o_Col;
void main(void)
{
    vec3 surfPos = f_Pos.xyz / f_Pos.w;
    vec3 surfNor = normalize(f_Nor);
	vec3 surfCol = texture(g_Texture, f_Tex).rgb;
	vec3 col = surfCol * g_AmbientColor.rgb;

    //Apply gamma correction
    //o_Color = vec4(pow(linearColor, vec3(1.0 / 2.2)), 1.0);

    o_Col = vec4(col, 1.0);
    o_Col = vec4(abs(f_Nor), 1.0);
}
