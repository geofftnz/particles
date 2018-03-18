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
	return randomPos01(particle,nonce)	* vec3(2.0) - vec3(1.0);
}

// random points on sphere surface, centered at origin, radius r
vec3 sphere(vec2 particle, float r)
{
	float rsq = r*r;
	vec3 p = randomPos(particle,1.0);

	if (dot(p,p)>1.0) p = randomPos(particle,1.0);
	if (dot(p,p)>1.0) p = randomPos(particle,2.0);
	if (dot(p,p)>1.0) p = randomPos(particle,3.0);
	if (dot(p,p)>1.0) p = randomPos(particle,4.0);

	p = normalize(p) * r;
	//p *= r;
	//p = normalize(p) * r * sqrt(hash(p));

	return p;	
}

vec4 blob1(vec2 coord)
{
	vec3 p;
	float s;

	float group = floor(coord.y * 16.0);
	float group2 = floor(coord.x * 2.0);

	p = sphere(coord,1.0);

	float ripple = 4.0 + sin(time * 7.6) * 0.5 + sin(time * 3.0) * 1.2 ;

	//ripple *= hash(group) + 1.0;

	p *= 1.0 + 
		sin(p.x * ripple * 1.3 + time*12.7 * hash(group2) ) * 0.05 + 
		sin(p.y * ripple * 1.7 + time*13.7 * hash(group2*3.7) ) * 0.05 + 
		sin(p.z * ripple * 1.9 + time*13.5 * hash(group2*7.2) ) * 0.05;

	//p *= 1.0 + sin(p.x * sin(p.y) * p.z * time * 0.1) * 0.02;

	p *= 0.1;

	//p.x += hash(group) * 0.5;
	//p.y += hash(group2) * 0.5;

	//s = sin(time * 4.0 + hash(particleIndex) * 17.0);
	//s = 2.0 + s * 1.5;
	s = 0.5;

	return vec4(p,s);
}

vec4 strings1(vec2 coord)
{
	vec3 p = vec3(0.0);

	vec2 qc = floor(coord*16.0);
	vec2 residual = coord - qc;

	p = randomPos01(qc,1.0);
	float n = hash(residual);

	float ofs = 0.0;
	p.x += sin(time * 2.3 + n * 17.0 + ofs * 7.1) * 0.1;
	p.y += sin(time * 1.1 + n * 15.0 + ofs * 3.0) * 0.1;
	p.z += sin(time * 1.7 + n * 13.0 + ofs * 5.0) * 0.1;

	return vec4(p,1.5);
}

//#define N 5000.0

vec3 getRandomPos(float x)
{
	return vec3(hash(x),hash(x * 7.5),hash(x* 33.6));
}
vec3 getNonRandomOffset(float x, float r)
{
	return vec3(sin(x+r),sin(x * 7.5+r),sin(x* 33.6+r));
}

vec4 strings2(vec2 coord)
{
	vec3 p = vec3(0.0);
	float index = coord.x / 1024.0 + coord.y;

	float N = (sin(time*0.1)*0.5+0.5) * 1000.0;

	vec2 qc = floor(coord*N);

	float grp = floor(index * N);
	float grpi = index * N - grp;

	float t = grpi;
	float t2 = t*t;
	float t3 = t2*t;

	float tangent = 1.0;

	//vec3 p0 = getRandomPos(grp-1);
	vec3 p0 = getRandomPos(grp);
	vec3 p1 = getRandomPos(grp+1.0);
	//vec3 p3 = getRandomPos(grp+2);
	vec3 m0 = (p1 - getRandomPos(grp-1.0))*tangent;
	vec3 m1 = -(p0 - getRandomPos(grp+2.0))*tangent;

	p = p0 * (2.0*t3-3.0*t2+1.0) + m0 * (t3-2.0*t2+t) + p1*(-2.0*t3+3.0*t2) + m1*(t3-t2);

	//p0 += getNonRandomOffset(grp,t) * 0.1;
	//p1 += getNonRandomOffset(grp+1,t) * 0.1;
	
	

	//p = normalize(p - vec3(0.5));


	return vec4(p,1.5);
}


// gets the accelleration vector field at p
vec3 field(vec3 p)
{
	vec3 v = vec3(0.0);

	//v.x = 0.001;
	//v = normalize(p) * -0.001;
	//p*= 5.;
	v.x = snoise(vec4(p,0.0));
	v.y = snoise(vec4(p,3.7));
	v.z = snoise(vec4(p,7.1));

	v += normalize(p) * -0.1;

	return v;
}


void main(void)
{
	vec4 pos = texture2D(particlePositionTexture,texcoord);
	vec4 vel = texture2D(particleVelocityTexture,texcoord);
	vec4 col = texture2D(particleColourTexture,texcoord);

	//pos = blob1(texcoord);
	//col = vec4(1.0,0.2,0.1,1.0);


	// pos: xyz + size
	// vel: xyz + life
	// col: rgb + ?

	if (vel.a <= 0.00001)
	{
		// respawn particle
		pos.xyz = randomPos(texcoord,time);
		pos.a = 5.0;
		vel.xyz = normalize(pos.xyz) * -0.00001;
		vel.a = 1.0; //hash13(vec3(pos.xy,time));
		col = vec4(randomPos01(texcoord,time*2.0),0.2);
	} 
	else
	{
		vel.xyz += field(pos.xyz) * 0.01;
		vel.xyz *= 0.99; // drag
		pos.xyz += vel.xyz * 0.01;
		vel.a -= 0.0001;
		col.rgb = normalize(vel.xyz) * 0.5 + 0.5;
		col.a = (vel.a * (1.0-vel.a)) * 4.0;
	}

	out_Pos = pos;
	out_Vel = vel;
	out_Col = col;
}
