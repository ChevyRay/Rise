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
uniform sampler2D g_Normal;
uniform sampler2D g_Position;
uniform vec3 g_CameraPosition;
uniform vec3 g_LightDirection;
uniform vec4 g_LightColor;
uniform vec4 g_SpecularColor;
uniform float g_Shininess;
in vec2 f_Tex;
layout(location = 0) out vec3 o_Col;
void main(void)
{
    vec3 surfacePosition = texture(g_Position, f_Tex).rgb;
    vec3 surfaceNormal = texture(g_Normal, f_Tex).rgb;

    //Diffuse
    float diffuse = max(0.0, dot(-g_LightDirection, surfaceNormal));
    vec3 lightColor = g_LightColor.rgb * diffuse;

    //Specular (Blinn-Phong)
    /*vec3 surfaceToCamera = normalize(g_CameraPosition - surfacePosition);
    vec3 halfDir = normalize(-g_LightDirection + surfaceToCamera);
    float specular = pow(max(dot(surfaceNormal, halfDir), 0.0), g_Shininess);
    lightColor += g_SpecularColor.rgb * g_LightColor.rgb * specular;*/

    o_Col = lightColor;
}
