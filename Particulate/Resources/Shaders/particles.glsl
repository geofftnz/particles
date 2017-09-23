//|vert
#version 450
precision highp float;
layout (location = 0) in vec2 vertex;
layout (location = 0) out vec3 pos;
layout (location = 1) out float size;
//layout (location = 1) out vec2 pos;
uniform float screenFactor;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;

void main() 
{
	//gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.x,0.0,vertex.y,1.0);
	//pos = gl_Position.xyz;
	//texcoord = vertex.xy;

	// https://stackoverflow.com/questions/8608844/resizing-point-sprites-based-on-distance-from-the-camera
	//float s = 2.0 * (sqrt(screenWidth) / 28.0);
	float s = 2.0 * screenFactor;

	vec4 p = vec4(vertex.x,vertex.y,mod(vertex.x*vertex.y*50.0,1.0),1.0);  //TODO read from texture.

	vec4 eyePos = viewMatrix * modelMatrix * p;
	vec4 corner = projectionMatrix * vec4(s,s,eyePos.z,eyePos.w);
	size = corner.x / corner.w;
	gl_Position = projectionMatrix * eyePos;
	gl_PointSize = size;
	pos = p.xyz;
}

//|frag
#version 450
precision highp float;
//layout (location = 0) in vec2 vertex;
layout (location = 0) in vec3 pos;
layout (location = 1) in float size;
layout (location = 0) out vec4 out_Colour;

void main(void)
{
	vec3 col = vec3(0.1) + vec3(pos);

	vec2 pc = (gl_PointCoord.st - vec2(0.5)) * 2.0;
	float rsq = dot(pc,pc);

	float a = (1.0 - smoothstep(0.5,1.0,rsq));
	a *= (1.0 / (1.0 + rsq * 50.0));

	a *= min(1.0,size*0.1);
	

	out_Colour = vec4(col,a);
}