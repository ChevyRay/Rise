#version 330
uniform mat4 ModelMatrix;
uniform mat4 ModelViewProjectionMatrix;
layout(location=0) in vec3 vertPosition;
layout(location=1) in vec3 vertNormal;
layout(location=2) in vec2 vertUV;
layout(location=3) in vec4 vertColor;
out vec3 fragNormal;
out vec2 fragUV;
out vec4 fragColor;
void main()
{
    gl_Position = ModelViewProjectionMatrix * vec4(vertPosition, 1.0);
    fragNormal = transpose(inverse(mat3(ModelMatrix))) * vertNormal;
    fragUV = vertUV;
    fragColor = vertColor;
}
...
#version 330
uniform sampler2D Texture;
in vec3 fragNormal;
in vec2 fragUV;
in vec4 fragColor;
layout(location = 0) out vec4 outColor;
layout(location = 1) out vec3 outNormal;
layout(location = 2) out float outDepth;
void main(void)
{
	outColor = texture(Texture, fragUV) * fragColor;
	outNormal = normalize(fragNormal);
	outDepth = gl_FragCoord.z;
}

/*vec2 packNormal(in vec3 normal)
{
	const float SCALE = 1.7777;
	float scalar1 = (normal.z + 1.0) * (SCALE * 2.0);
	return normal.xy / scalar1 + 0.5;
}

vec3 unpackNormal(in vec2 enc)
{
	const float SCALE = 1.7777;
	vec2 nn = enc * (2.0 * SCALE) - SCALE;
	float g = 2.0 / (dot(nn.xy, nn.xy) + 1.0);
	vec3 normal;
	normal.xy = g * nn.xy;
	normal.z = g - 1.0;
	return normalize(normal);
}*/
