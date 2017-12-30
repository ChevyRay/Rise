#version 330
uniform mat4 ModelViewProjectionMatrix;
layout(location=0) in vec3 vertPosition;
void main()
{
    gl_Position = ModelViewProjectionMatrix * vec4(vertPosition, 1.0);
}
...
#version 330
layout(location = 0) out float outDepth;
void main(void)
{
	outDepth = gl_FragCoord.z;
}
