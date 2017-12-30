//#include 3d_header_vert.glsl
//#include 3d_uniforms.glsl

void main()
{
    gl_Position = ModelViewProjectionMatrix * vec4(vertPosition, 1.0);
    fragUV = vertUV;
    fragColor = vertColor;
}
