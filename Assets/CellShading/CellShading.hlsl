#ifndef CELL_SHADING
#define CELL_SHADING

#include "./Noise.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _SpecColor;
half4 _EmissionColor;
half _Cutoff;
half _Smoothness;
half _BumpScale;
half _OcclusionStrength;
half _LightColorWeight;
half _AccentLightWeight;
half _AccentLightWidth;
half _StepSmoothness;
half _HeightMapDepth;
half _HeightMapFidelity;

half3 _LightRampColors0;
half3 _LightRampColors1;
half3 _LightRampColors2;
half3 _LightRampColors3;
half3 _LightRampColors4;
half3 _LightRampColors5;
half3 _LightRampColors6;
half3 _LightRampColors7;

half _LightRampValues0;
half _LightRampValues1;
half _LightRampValues2;
half _LightRampValues3;
half _LightRampValues4;
half _LightRampValues5;
half _LightRampValues6;
half _LightRampValues7;

float _LightRampSmoothness;
float _LightRampSize;

half _DotSize;
half _DotRotation;

half _HatchingFrequency;
half _HatchingWidth;
half _HatchingOffsetPerLayer;
half _HatchingRotation;
half _HatchingCross;

half _RenderMode;
CBUFFER_END

TEXTURE2D(_HeightMap);          SAMPLER(sampler_HeightMap);
TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_SpecGlossMap);       SAMPLER(sampler_SpecGlossMap);

float CellSpecular(in float3 light, in float3 normal, in float3 view, in float smoothness) {
    return length(LightingSpecular(1.0, light, normalize(normal), view, float4(1.0, 1.0, 1.0, 0.0), exp2(10 * smoothness + 1)));
}

float CellLighting(in float3 light, in float3 normal, in float occlusion, in float atten, in float intensity) {
    return saturate((dot(light, normal) + 1.0) * 0.5 * occlusion * atten * intensity);
}

void CellFullLighting(
    in float3 normal, in float3 light, in float intensity,
    in float3 view, in float occlusion, in float atten, in float smoothness,
    inout float diffuse, inout float spec
) {
    diffuse += CellLighting(light, normal, occlusion, atten, intensity);
    spec += CellSpecular(light, normal, view, smoothness) * intensity;
}

float SmoothStep(float v, float t, float smoothness) {
    float scale = 64.0 * (1.0 - smoothness);
    v = saturate(scale * (v - t));
    float smooth = saturate((v * v) * (3.0 - (2.0 * v)));
    
    return (v * smoothness) + (smooth * (1.0 - smoothness));
}

float4 CellAlbedo(float2 uv, float4 tint, float3 lightTint, float lightInfluence, TEXTURE2D_PARAM(colorMap, sampler_colorMap)) {
    float4 color = SAMPLE_TEXTURE2D(colorMap, sampler_colorMap, uv);
    color.rgb *= tint.rgb;
	lightTint = SafeNormalize(lightTint) * 1.73205080757;
    return lerp(color, color * float4(lightTint, color.a), lightInfluence);
}

float2 ParallaxMapping(float2 uv, float3 viewDir, float layers, TEXTURE2D_PARAM(heightMap, sampler_heightMap)) {
    #ifdef _HEIGHTMAP
        float2 dir = normalize( viewDir.xy );

        float fl = length( viewDir );
        float dl = 1.0 / layers;
        float2 P = dir * (sqrt( fl * fl - viewDir.z * viewDir.z ) / viewDir.z) * _HeightMapDepth;
        float2 delta = dl * P;
        float2 cuv = uv;
        float2 dx = ddx(uv);
        float2 dy = ddy(uv);

        float ch = 0.0;
        float ph = 1.0;
        float chb = 1.0;
        float h = 0.0;
    
        float2 pt1 = 0;
        float2 pt2 = 0;
    
        for (int nStepIndex = 0; nStepIndex < layers; nStepIndex++) {
            cuv -= delta;
            ch = SAMPLE_TEXTURE2D_GRAD(heightMap, sampler_heightMap, cuv, dx, dy).r;
            chb -= dl;
            if ( ch > chb )  {   
                pt1 = float2( chb, ch );
                pt2 = float2( chb + dl, ph );
                nStepIndex = layers + 1;
            }
            ph = ch;
        }

        float delta1 = pt1.x - pt1.y;  
        float delta2 = pt2.x - pt2.y;
        float denom = delta2 - delta1;
        if ( denom == 0.0f ) {
            h = 0.0f;
        }
        else {
            h = (pt1.x * delta2 - pt2.x * delta1 ) / denom;
        }
    
        return uv - (P * (1 - h));
    #else
        return uv;
    #endif
}

half SampleOcclusion(float2 uv) {
    #ifdef _OCCLUSIONMAP
        #if defined(SHADER_API_GLES)
            return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
        #else
            half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
            return lerp(1.0, occ, _OcclusionStrength);
        #endif
    #else
        return 1.0;
    #endif
}

int SampleLightRamp(float diffuse, out half valueA, out half valueB, out half3 colorA, out half3 colorB) {
	if (diffuse < _LightRampValues0 || _LightRampSize < 2) {
		valueA = 0;
		valueB = _LightRampValues0;
		colorA = _LightRampColors0;
		colorB = _LightRampColors0;
		return 1;
	}

	if (diffuse < _LightRampValues1 || _LightRampSize < 3) {
		valueA = _LightRampValues0;
		valueB = _LightRampValues1;
		colorA = _LightRampColors0;
		colorB = _LightRampColors1;
		return 2;
	}

	if (diffuse < _LightRampValues2 || _LightRampSize < 4) {
		valueA = _LightRampValues1;
		valueB = _LightRampValues2;
		colorA = _LightRampColors1;
		colorB = _LightRampColors2;
		return 3;
	}
	
	if (diffuse < _LightRampValues3 || _LightRampSize < 5) {
		valueA = _LightRampValues2;
		valueB = _LightRampValues3;
		colorA = _LightRampColors2;
		colorB = _LightRampColors3;
		return 4;
	}

	if (diffuse < _LightRampValues4 || _LightRampSize < 6) {
		valueA = _LightRampValues3;
		valueB = _LightRampValues4;
		colorA = _LightRampColors3;
		colorB = _LightRampColors4;
		return 5;
	}

	if (diffuse < _LightRampValues5 || _LightRampSize < 7) {
		valueA = _LightRampValues4;
		valueB = _LightRampValues5;
		colorA = _LightRampColors4;
		colorB = _LightRampColors5;
		return 6;
	}

	if (diffuse < _LightRampValues6 || _LightRampSize < 8) {
		valueA = _LightRampValues5;
		valueB = _LightRampValues6;
		colorA = _LightRampColors5;
		colorB = _LightRampColors6;
		return 7;
	}

	if (diffuse < _LightRampValues7) {
		valueA = _LightRampValues6;
		valueB = _LightRampValues7;
		colorA = _LightRampColors6;
		colorB = _LightRampColors7;
		return 8;
	}

	valueA = _LightRampValues7;
	valueB = max(_LightRampValues7, 1.0);
	colorA = _LightRampColors7;
	colorB = _LightRampColors7;
	return 9;
}

half4 FinalBandsCalculation(
	float3 normalWS, float3 viewWS, float2 uv,
	float diffuse, float spec, float smoothness, float fog,
	float3 specular, float3 light, float3 emission,
	TEXTURE2D_PARAM(colorMap, sampler_colorMap)
) {
	half vA = 0; half vB = 0;
	half3 cA = half3(0,0,0); half3 cB = half3(0,0,0);

	// diffuse lighting
	int level = SampleLightRamp(diffuse, vA, vB, cA, cB);
	half pos = saturate((diffuse - vA) / max(abs(vB - vA), 0.0001));
	half w = pow(256, 1 - _LightRampSmoothness);

	float3 lighting = lerp(cA, cB, saturate((w * pos) - w + 1.0));

	// specular lighting
	lighting *= 1.0 + (specular * SmoothStep(spec, saturate(1.0 - smoothness), _StepSmoothness) * 6.0);
	
    float4 finalColor = CellAlbedo(uv, _BaseColor, light, _LightColorWeight, TEXTURE2D_ARGS(colorMap, sampler_colorMap));
    finalColor.rgb *= lighting;

	// accent lighting
	float fresnel = pow(1.0 - saturate(dot(normalWS, SafeNormalize(viewWS))), saturate(1.0 - _AccentLightWidth) * 8.0);
    finalColor.rgb *= 1.0 + (SmoothStep(fresnel, 1.0 - _AccentLightWeight, _StepSmoothness) * 6.0 * _AccentLightWeight);

    finalColor.rgb += emission;

    finalColor.rgb = MixFog(finalColor.rgb, fog);
    return half4(finalColor.rgb, Alpha(finalColor.a, _BaseColor, _Cutoff));
}

half4 FinalDotsCalculation(
	float3 normalWS, float3 viewWS, float2 uv, float2 sp,
	float diffuse, float spec, float smoothness, float fog,
	float3 specular, float3 light, float3 emission,
	TEXTURE2D_PARAM(colorMap, sampler_colorMap)
) {
	half radians = _DotRotation * 6.28318530718;
	half cosR = cos(radians); half sinR = sin(radians);
	sp -= 0.5;
	sp = float2(sp.x * cosR - sp.y * sinR, sp.y * cosR + sp.x * sinR);
	sp += 0.5;

	half dScale = 100.0 / _DotSize;
	half dVal = sin(sp.x * dScale) + sin(sp.y * dScale);
	
	// diffuse lighting
	half vA = 0; half vB = 0;
	half3 cA = half3(0,0,0); half3 cB = half3(0,0,0);

	int level = SampleLightRamp(diffuse, vA, vB, cA, cB);
	half pos = saturate((diffuse - vA) / max(abs(vB - vA), 0.0001));
	half w = pow(256, 1 - _LightRampSmoothness);
	w = saturate((w * pos) - w + 1.0);

	float3 lighting = lerp(cA, cB, step(dVal, saturate(diffuse * w + pow(w, 8)) * 4.0 - 2.0));

	// specular lighting
	lighting *= 1.0 + (step(dVal, spec * 4.0 - 2.0) * specular * 6.0);

	//accent lighting
	float fresnel = pow(1.0 - saturate(dot(normalWS, SafeNormalize(viewWS))), saturate(1.0 - _AccentLightWidth) * 8.0);
	float rim = SmoothStep(fresnel, 1.0 - _AccentLightWeight, sqrt(saturate(_StepSmoothness + 0.2)));
	float accent = 1.0 + (step(dVal, rim * 4.0 - 2.0) * 2.0 * _AccentLightWeight);

	
    float4 finalColor = CellAlbedo(uv, _BaseColor, light, _LightColorWeight, TEXTURE2D_ARGS(colorMap, sampler_colorMap));
    finalColor.rgb *= lighting;
    finalColor.rgb *= accent;
    finalColor.rgb += emission;

    finalColor.rgb = MixFog(finalColor.rgb, fog);
    return half4(finalColor.rgb, Alpha(finalColor.a, _BaseColor, _Cutoff));
}

half4 FinalCrossHatchCalculation(
	float3 normalWS, float3 viewWS, float2 uv, float2 sp, float depth,
	float diffuse, float spec, float smoothness, float fog,
	float3 specular, float3 light, float3 emission,
	TEXTURE2D_PARAM(colorMap, sampler_colorMap)
) {
	half radians = _HatchingRotation * 6.28318530718;
	half cosR = cos(radians); half sinR = sin(radians);
	sp -= 0.5;
	sp = float2(sp.x * cosR - sp.y * sinR, sp.y * cosR + sp.x * sinR);
	sp += 0.5;
	
	float angle = snoise(float3(sp.x, 0, 0) * 8) * 6.3;
	sp += float2(cos(angle), sin(angle)) * 0.001;

	sp.x += snoise(float3(sp.y, 0, 0) * 2) * 0.01;

	// specular lighting
	half3 specularLight = spec * specular * 6.0 + 1.0;

	// accent lighting
	float fresnel = pow(1.0 - saturate(dot(normalWS, SafeNormalize(viewWS))), saturate(1.0 - _AccentLightWidth) * 8.0);
	float accent = 1.0 + (fresnel * 2.0 * _AccentLightWeight);
	
	// lighting
	half brightness = diffuse * specularLight * accent;
	half vA = 0; half vB = 0;
	half3 cA = half3(0,0,0); half3 cB = half3(0,0,0);
	int level = SampleLightRamp(brightness, vA, vB, cA, cB);
	half pos = saturate((brightness - vA) / max(abs(vB - vA), 0.0001));
	half softEdge = saturate(pow(pos, 1 + 128 * (1.0 - _LightRampSmoothness)));

	float freq = 1.0 / pow(1024.0, _HatchingFrequency - 0.3) / depth;
	float width = pow(max(_ScreenParams.x, _ScreenParams.y) * 1600, (1.0 - _HatchingWidth)) * depth;
	//float offset = _HatchingOffsetPerLayer * freq;

	float pattern = 1.0;
	int softened = 0;
	for (int i = _LightRampSize - level; i > 0; i--) {
		int divs = (1 << (i - 1));
		float offset = freq / divs;
		
		int extent = divs * 0.5;
		for (int j = -extent; j <= extent; j++) {
			float lines = fmod(sp.x + offset * j, freq) - (freq * 0.5);
			
			float w = width;
			if (softened == 0) w *= pow(lerp(1, 32, softEdge), 0.5);
			
			lines = pow(saturate(w * lines * lines), 8 * _StepSmoothness);

			if (_HatchingCross > 0.5) {
				float cross = fmod(sp.y + offset * j, freq) - (freq * 0.5); 
				cross = pow(saturate(w * cross * cross), 8 * _StepSmoothness);
				lines *= cross;
			}
		
			if (softened == 0) pattern *= lerp(lines, 1, softEdge);
			else pattern *= lines;
		}
		
		softened = 1;
	}
	
    float4 finalColor = CellAlbedo(uv, _BaseColor, light, _LightColorWeight, TEXTURE2D_ARGS(colorMap, sampler_colorMap));
    finalColor.rgb *= pattern;
    finalColor.rgb += emission;

    finalColor.rgb = MixFog(finalColor.rgb, fog);
    return half4(finalColor.rgb, Alpha(finalColor.a, _BaseColor, _Cutoff));
}

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED