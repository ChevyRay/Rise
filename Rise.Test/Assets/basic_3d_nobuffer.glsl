#version 330
uniform mat4 ModelViewProjectionMatrix;
layout(location=0) in vec3 vertPosition;
layout(location=2) in vec2 vertUV;
layout(location=3) in vec4 vertColor;
out vec2 fragUV;
out vec4 fragColor;
void main()
{
    gl_Position = ModelViewProjectionMatrix * vec4(vertPosition, 1.0);
    fragUV = vertUV;
    fragColor = vertColor;
}
...
#version 330
uniform sampler2D Texture;
in vec2 fragUV;
in vec4 fragColor;
layout(location = 0) out vec4 outColor;
void main(void)
{
	outColor = texture(Texture, fragUV) * fragColor;
}
