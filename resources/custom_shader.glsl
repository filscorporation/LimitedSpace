#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 texCoordAtr;

out vec4 vertexColor;
out vec2 texCoord;

uniform mat4 view_projection;

void main()
{
    gl_Position = view_projection * vec4(position, 1.0f);
    vertexColor = color;
    texCoord = texCoordAtr;
}

#version 330 core

in vec4 vertexColor;
in vec2 texCoord;

out vec4 color;

uniform sampler2D main_tex;
uniform int override_color_flag;
uniform int override_alpha_flag;
uniform vec4 override_color;

void main()
{
    vec4 texel = texture(main_tex, texCoord);
    color = texel * vertexColor;
    if (override_color_flag == 1)
        color = vec4(override_color.rgb, color.a);
    if (override_alpha_flag == 1)
        color = vec4(color.rgb, color.a * override_color.a);
    if (color.a < 0.01) discard;
}
