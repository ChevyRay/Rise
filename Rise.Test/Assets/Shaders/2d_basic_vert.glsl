//#include 2d_header_vert.glsl
//#include 2d_uniforms.glsl

void main(void)
{
    gl_Position = Matrix * vec4(vertPosition, 0.0, 1.0);
    fragUV = vertUV;
    fragMultiply = vertMultiply;
    fragAdd = vertAdd;
}
