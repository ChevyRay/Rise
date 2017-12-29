#version 330
uniform mat4 Matrix;
layout(location = 0) in vec2 vertPosition;
layout(location = 1) in vec2 vertUV;
out vec2 fragUV;
void main()
{
    gl_Position = Matrix * vec4(vertPosition, 0.0, 1.0);
    fragUV = vertUV;
}
...
#version 330
uniform sampler2D ColorMap;
uniform sampler2D NormalMap;
uniform sampler2D PositionMap;
uniform sampler2D SpecularMap;

uniform vec3 CameraPosition;

uniform int LightType;
uniform vec3 LightColor;
uniform vec3 LightPosition;
uniform vec3 LightDirection;
uniform float LightSize;
uniform float LightLength;
uniform mat4 LightMatrix;

in vec2 fragUV;

layout(location = 0) out vec4 outColor;

void main()
{
    vec3 position = texture(PositionMap, fragUV).xyz;
    vec3 directionToLight;
    float lightness = 1.0;

    if (LightType == 2)
    {
        //Directional light
        directionToLight = -LightDirection;
        //lightness = shadowFactor(LightMatrix * position);
    }
    else
    {
        //Point light
        directionToLight = normalize(LightPosition - position);
        float distanceToLight = length(LightPosition - position);

        //Check if fragment is inside the light radius
        if (distanceToLight < LightSize)
        {
            lightness = 1.0 - distanceToLight / LightSize;

            //Cone light
            if (LightType == 1)
            {
                //Check if fragment is inside the cone angle
                if (acos(dot(-directionToLight, LightDirection)) < LightLength)
                {
                    //lightness *= shadowFactor(LightMatrix * position);
                }
                else
                {
                    lightness = 0.0;
                }
            }
            else
            {
                //lightness *= shadowCubeFactor(i, normalize(position - LightPosition));
            }
        }
    }

    vec3 normal = texture(NormalMap, fragUV).xyz;
    float diffuse = max(dot(directionToLight, normal), 0.0);

    vec3 color = texture(ColorMap, fragUV).rgb;
    vec3 lightColor = color * LightColor * diffuse;

    if (diffuse > 0.0)
    {
        vec3 directionToCamera = normalize(CameraPosition - position);
        vec3 halfDirection = normalize(directionToLight + directionToCamera);
        vec4 specular = texture(SpecularMap, fragUV);
        float specularAngle = max(0.0, dot(normal, halfDirection));
        lightColor += color * specular.rgb * LightColor * pow(specularAngle, specular.a);
        //Specular rgb (color) a (shininess)
    }

    //Apply gamma correction
    outColor = vec4(pow(lightColor * lightness, vec3(1.0 / 2.2)), 1.0);
}
