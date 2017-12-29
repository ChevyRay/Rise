#version 330
layout(location = 0) in vec2 vertPosition;
layout(location = 1) in vec2 vertUV;
layout(location = 2) in vec4 vertMultiply;
layout(location = 3) in vec4 vertAdd;
out vec3 fragPosition;
out vec2 fragUV;
out vec4 fragMultiply;
out vec4 fragAdd;
