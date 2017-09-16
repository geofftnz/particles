//|vert
#version 410
precision highp float;
layout (location = 0) in vec2 vertex;
layout (location = 0) out vec3 pos;
//layout (location = 1) out vec2 pos;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;

void main() 
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.xy,0.0,1.0);
	pos = gl_Position.xyz;
	//texcoord = vertex.xy;

	gl_PointSize = 8.0;
}

//|frag
#version 410
precision highp float;
//layout (location = 0) in vec2 vertex;
layout (location = 0) in vec3 pos;
layout (location = 0) out vec4 out_Colour;

void main(void)
{

	out_Colour = pos * 0.5 + 0.5;
}