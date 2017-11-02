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

uniform float time;

//From Dave (https://www.shadertoy.com/view/XlfGWN)
float hash13(vec3 p){
	p  = fract(p * vec3(.16532,.17369,.15787));
    p += dot(p.xyz, p.yzx + 19.19);
    return fract(p.x * p.y * p.z);
}
float hash(float x){	return fract(cos(x*124.123)*412.0); }
float hash(vec2 x){	return fract(cos(dot(x.xy,vec2(2.31,53.21))*124.123)*412.0); }


vec3 randomPos(vec2 particle, float nonce)
{	
	return vec3(
		hash13(vec3(particle,hash(nonce))),
		hash13(vec3(particle*133.7,hash(nonce))),
		hash13(vec3(particle*1393.7,hash(nonce)))
	) * vec3(2.0) - vec3(1.0);
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

	return p;	
}

void main(void)
{
	vec3 p;
	float s;
	float particleIndex = texcoord.x + texcoord.y / 1024.0;

	float group = floor(texcoord.y * 4.0);
	float group2 = floor(texcoord.x * 2.0);

	p = sphere(texcoord,1.0);

	float ripple = 4.0 + sin(time * 0.6) * 2.0;

	//ripple *= hash(group) + 1.0;

	p *= 1.0 + 
		sin(p.x * ripple * 1.3 + time*2.7 * hash(group2) ) * 0.05 + 
		sin(p.y * ripple * 1.7 + time*3.7 * hash(group2*3.7) ) * 0.05 + 
		sin(p.z * ripple * 1.9 + time*3.5 * hash(group2*7.2) ) * 0.05;

	//p *= 1.0 + sin(p.x * sin(p.y) * p.z * time * 0.1) * 0.02;

	p *= 0.1;

	//p.x += hash(group) * 0.5;
	//p.y += hash(group2) * 0.5;

	//s = sin(time * 4.0 + hash(particleIndex) * 17.0);
	//s = 2.0 + s * 1.5;
	s = 0.5;

	out_Pos = vec4(p,s);
}