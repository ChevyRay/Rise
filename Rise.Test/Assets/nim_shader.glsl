#version 330

uniform mat4 in_ModelViewProjection;
uniform mat4 in_Model;

layout(location=0) in vec3 in_Position;
layout(location=1) in vec3 in_Normal;
layout(location=2) in vec2 in_TexCoord;
layout(location=3) in vec4 in_Color;

out vec4 frag_Position;
out vec3 frag_Normal;
out vec2 frag_TexCoord;
out vec4 frag_Color;

void main()
{
    vec4 pos = vec4(in_Position, 1.0);
    gl_Position = in_ModelViewProjection * pos;
    frag_Position = in_Model * pos;
    frag_Normal = transpose(inverse(mat3(in_Model))) * in_Normal;
    frag_TexCoord = in_TexCoord;
    frag_Color = in_Color;
}

...

#version 330

uniform sampler2D in_Texture;
uniform vec3 in_CameraPosition;
uniform vec3 in_AmbientColor;
uniform vec3 in_SpecularColor;
uniform float in_Shininess;

#define MAX_LIGHTS " + Shader.MaxLights + @"
uniform int in_LightCount;
uniform struct Light {
    int type;
    vec3 position;
    vec3 color;
    float radius;
    float coneAngle;
    vec3 direction;
    mat4 matrix;
} in_Lights[MAX_LIGHTS];
uniform sampler2D in_ShadowMaps[MAX_LIGHTS];
//uniform samplerCube in_ShadowCubes[MAX_LIGHTS];
uniform samplerCube in_ShadowCube;

in vec4 frag_Position;
in vec3 frag_Normal;
in vec2 frag_TexCoord;
in vec4 frag_Color;

out vec4 out_Color;

float shadowFactor(int i, vec4 pos)
{
    vec3 proj = pos.xyz / pos.w;
    vec2 uv;
    uv.x = 0.5 * proj.x + 0.5;
    uv.y = 0.5 * proj.y + 0.5;
    float z = 0.5 + proj.z * 0.5;
    float depth = texture(in_ShadowMaps[i], uv).r;

    if (depth < z)
    return 0.0;
    else
    return 1.0;

    //non-branching alternative? if above is not optimized out
    //float factor = Depth - z;
    //return min(1.0, max(factor, 0.0) / abs(factor));
}

float shadowCubeFactor(int i, vec3 lightToSurface)
{
    float SampledDistance = texture(in_ShadowCube, lightToSurface).r;

    float Distance = length(lightToSurface);

    if (Distance < SampledDistance + 0.0001)
    return 1.0;
    else
    return 0.0;
}

void main()
{
    vec3 surfacePos = frag_Position.xyz / frag_Position.w;
    vec3 surfaceNormal = normalize(frag_Normal);
    vec3 surfaceColor = texture(in_Texture, frag_TexCoord).rgb;

    vec3 linearColor = surfaceColor * in_AmbientColor;

    //Apply all the lights
    for (int i = 0; i < in_LightCount; ++i)
    {
        Light light = in_Lights[i];

        float fade = 0.0;
        vec3 surfaceToLight;

        if (light.type == 2)
        {
            //Directional light
            surfaceToLight = -light.direction;
            fade = shadowFactor(i, light.matrix * frag_Position);
        }
        else
        {
            //Point light
            surfaceToLight = normalize(light.position - surfacePos);
            float distanceToLight = length(light.position - surfacePos);

            if (distanceToLight < light.radius)
            {
                fade = 1.0 - distanceToLight / light.radius;

                //Cone light
                if (light.type == 1)
                {
                    if (light.coneAngle < acos(dot(-surfaceToLight, light.direction)))
                    {
                        fade = 0.0;
                    }
                    else
                    {
                        fade *= shadowFactor(i, light.matrix * frag_Position);
                    }
                }
                else
                {
                    fade *= shadowCubeFactor(i, surfacePos - light.position);
                }
            }
        }

        //Diffuse
        float diffuse = max(0.0, dot(surfaceToLight, surfaceNormal));
        vec3 lightColor = surfaceColor * light.color * diffuse;

        //Specular
        if (diffuse > 0.0)
        {
            vec3 surfaceToCamera = normalize(in_CameraPosition - surfacePos);

            //Blinn-Phong
            vec3 halfwayDir = normalize(surfaceToLight + surfaceToCamera);
            float specular = pow(max(dot(surfaceNormal, halfwayDir), 0.0), in_Shininess);

            //Phong
            //vec3 reflectDir = reflect(-surfaceToLight, surfaceNormal);
            //float specular = pow(max(dot(surfaceToCamera, reflectDir), 0.0), in_Shininess);

            //Apply specular
            lightColor += surfaceColor * in_SpecularColor * light.color * specular;
        }

        //Apply the light
        linearColor += lightColor * fade;
    }

    //Apply gamma correction
    out_Color = vec4(pow(linearColor, vec3(1.0 / 2.2)), 1.0);
}
