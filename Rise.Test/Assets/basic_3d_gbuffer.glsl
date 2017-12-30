#version 330
uniform mat4 ModelMatrix;
uniform mat4 ModelViewProjectionMatrix;
layout(location=0) in vec3 vertPosition;
layout(location=1) in vec3 vertNormal;
layout(location=2) in vec2 vertUV;
layout(location=3) in vec4 vertColor;
out vec3 fragPosition;
out vec3 fragNormal;
out vec2 fragUV;
out vec4 fragColor;
void main()
{
	vec4 pos = vec4(vertPosition, 1.0);
    gl_Position = ModelViewProjectionMatrix * pos;
    pos = ModelMatrix * pos;
	fragPosition = pos.xyz / pos.w;
    fragNormal = transpose(inverse(mat3(ModelMatrix))) * vertNormal;
    fragUV = vertUV;
    fragColor = vertColor;
}
...
#version 330
uniform sampler2D Texture;
in vec3 fragPosition;
in vec3 fragNormal;
in vec2 fragUV;
in vec4 fragColor;
layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out float outZ;
void main(void)
{
	outColor = texture(Texture, fragUV) * fragColor;
	outNormal = normalize(fragNormal);
	outZ = gl_FragCoord.z;
}
