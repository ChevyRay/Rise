//#include 2d_header_frag.glsl
//#include 2d_uniforms.glsl

layout(location = 0) out vec4 outColor;

void main(void)
{
    vec4 color = texture(Texture, fragUV);
    outColor = color * fragMultiply + fragAdd * color.a;
}
