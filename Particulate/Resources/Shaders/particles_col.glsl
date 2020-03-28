//|vert
#version 450
precision highp float;
layout (location = 0) in vec2 vertex;
layout (location = 0) out vec3 pos;
layout (location = 1) out float size;
layout (location = 2) out vec4 col;

uniform float screenFactor;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform sampler2D particlePositionTexture;
uniform sampler2D particleColourTexture;

void main() 
{
	// https://stackoverflow.com/questions/8608844/resizing-point-sprites-based-on-distance-from-the-camera
	//float s = 2.0 * (sqrt(screenWidth) / 28.0);

	vec4 ptex = texture2D(particlePositionTexture,vertex.xy);
	col = texture2D(particleColourTexture,vertex.xy);

	vec4 p = vec4(ptex.xyz,1.0);
	float s = ptex.a * screenFactor;

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
layout (location = 2) in vec4 col;
layout (location = 0) out vec4 out_Colour;

void main(void)
{
	vec2 pc = (gl_PointCoord.st - vec2(0.5)) * 2.0;
	float rsq = dot(pc,pc);

	vec4 col2 = col; //vec4(1.0,0.2,0.1,1.0);

	// circle pattern in alpha
	float a = max(0.0,(1.0 - rsq));
	//a*=a;
	a = step(0.06,a);
	
	// size < 0 alpha falloff
	a *= min(1.0,size*0.8);

	col2.a *= clamp(a,0.0,1.0);

	out_Colour = col2;
}