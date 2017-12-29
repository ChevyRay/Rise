//#include 3d_header_vert.glsl
//#include 3d_uniforms.glsl

void main()
{
	vec4 pos = vec4(vertPosition, 1.0);
    gl_Position = ModelViewProjectionMatrix * pos;
    pos = ModelMatrix * pos;
	fragPos = pos.xyz / pos.w;
    fragNormal = transpose(inverse(mat3(ModelMatrix))) * vertNormal;
    fragUV = vertUV;
    fragColor = vertColor;
}
