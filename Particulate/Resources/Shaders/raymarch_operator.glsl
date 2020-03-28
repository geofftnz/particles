//|vert
#version 450
precision highp float;
layout (location = 0) in vec2 vertex;
layout (location = 0) out vec2 texcoord;

void main() 
{
	gl_Position = vec4(vertex.xy,0.0,1.0);
	texcoord = vertex.xy * 0.5 + 0.5;
}

//|frag
#version 450
precision highp float;
layout (location = 0) in vec2 texcoord;
layout (location = 0) out vec4 out_Pos;
layout (location = 1) out vec4 out_Vel;
layout (location = 2) out vec4 out_Col;

uniform float time;
uniform sampler2D particlePositionTexture;
uniform sampler2D particleVelocityTexture;
uniform sampler2D particleColourTexture;

#include "Common/Noise/noise4d.glsl"
#include "Common/hg_sdf.glsl"

//From Dave (https://www.shadertoy.com/view/XlfGWN)
float hash13(vec3 p){
	p  = fract(p * vec3(.16532,.17369,.15787));
    p += dot(p.xyz, p.yzx + 19.19);
    return fract(p.x * p.y * p.z);
}
float hash(float x){	return fract(cos(x*124.123)*412.0); }
float hash(vec2 x){	return fract(cos(dot(x.xy,vec2(2.31,53.21))*124.123)*412.0); }
float hash(vec3 x){	return fract(cos(dot(x.xyz,vec3(2.31,53.21,17.7))*124.123)*412.0); }

vec3 randomPos01(vec2 particle, float nonce)
{	
	return vec3(
		hash13(vec3(particle,hash(nonce))),
		hash13(vec3(particle*133.7,hash(nonce))),
		hash13(vec3(particle*1393.7,hash(nonce)))
	) ;
}
vec3 randomPos(vec2 particle, float nonce)
{
	return randomPos01(particle,nonce) * 2.0 - 1.0;
}

// random points on sphere surface, centered at origin, radius r
vec3 random3Sphere(vec2 particle, float r)
{
	float rsq = r*r;
	vec3 p = randomPos(particle,1.0);

	if (dot(p,p)>1.0) p = randomPos(particle,1.0);
	if (dot(p,p)>1.0) p = randomPos(particle,2.0);
	if (dot(p,p)>1.0) p = randomPos(particle,3.0);
	if (dot(p,p)>1.0) p = randomPos(particle,4.0);

	p = normalize(p) * r;
	return p;	
}



float sdf(vec3 p)
{
	float d = 1000.; 

	d = min(d,fPlane(p,vec3(0.0,-1.0,0.0),0.23));

	//vec3 p1 = p;
	//pMod2(p1.xz,vec2(0.3));
	//p.y -= 0.2;
	//d = min(d,fSphere(p1 - vec3(0.0,0.0,0.0), 0.02));
	d = min(d,fSphere(p - vec3(0.1,0.2,0.2), 0.02));

	//d = min(d,fSphere(p - vec3(-0.2,0.3,0.2), 0.05));

	//d = min(d,fPlane(p,vec3(0.0,-1.0,0.0),0.3));

	return d;
}

float intersect(vec3 p, vec3 v, float t0, float range, float emin)
{
	float t=t0;
	float jump = 0.;

	do
	{
		t += jump;
		jump = sdf(p + v * t);
	}
	while(t < range && jump > emin * t);

	return t;
}

void main(void)
{
	vec4 pos = texture2D(particlePositionTexture,texcoord);
	vec4 vel = texture2D(particleVelocityTexture,texcoord);
	vec4 col = texture2D(particleColourTexture,texcoord);

	// fix start position
	vec3 p = vec3(0.0);

	// random direction for trace
	vec3 rd = random3Sphere(texcoord + vec2(hash(time),hash(time*13.684)),1.0);

	float t = intersect(p,rd,0.,50.,0.0001);

	pos.xyz = p + rd * t;
	pos.a = 0.2;

	col = vec4(0.0);
	if (t < 40.){
		col = vec4(1.0,0.2,0.02,1.0 / (1.0 + t));
	}

	vel = vec4(0.0);
	

	out_Pos = pos;
	out_Vel = vel;
	out_Col = col;
}
