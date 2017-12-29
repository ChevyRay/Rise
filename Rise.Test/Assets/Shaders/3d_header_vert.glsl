#version 330
layout(location=0) in vec3 vertPosition;
layout(location=1) in vec3 vertNormal;
layout(location=2) in vec2 vertUV;
layout(location=3) in vec4 vertColor;
out vec3 fragPosition;
out vec3 fragNormal;
out vec2 fragUV;
out vec4 fragColor;
