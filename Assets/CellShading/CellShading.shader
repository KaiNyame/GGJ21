Shader "Universal Render Pipeline/Custom/Toon Lit"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (0.5,0.5,0.5,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        _LightColorWeight("Light Color Influence", Range(0.0, 1.0)) = 0.5
        _LightRampSize("Light Ramp Size", Float) = 4
        _LightRampSmoothness("Light Ramp Smoothness", Float) = 0
        
        _LightRampColors0("Light Ramp Colors [0]", Color) = (0.45, 0.45, 0.45)
        _LightRampColors1("Light Ramp Colors [1]", Color) = (0.65, 0.65, 0.65)
        _LightRampColors2("Light Ramp Colors [2]", Color) = (1.0, 1.0, 1.0)
        _LightRampColors3("Light Ramp Colors [3]", Color) = (1.0, 1.0, 1.0)
        _LightRampColors4("Light Ramp Colors [4]", Color) = (1.0, 1.0, 1.0)
        _LightRampColors5("Light Ramp Colors [5]", Color) = (1.0, 1.0, 1.0)
		_LightRampColors6("Light Ramp Colors [6]", Color) = (1.0, 1.0, 1.0)
		_LightRampColors7("Light Ramp Colors [7]", Color) = (1.0, 1.0, 1.0)
        
        _LightRampValues0("Light Ramp Values [0]", Float) = 0.1
        _LightRampValues1("Light Ramp Values [1]", Float) = 0.45
        _LightRampValues2("Light Ramp Values [2]", Float) = 0.8
        _LightRampValues3("Light Ramp Values [3]", Float) = 1
        _LightRampValues4("Light Ramp Values [4]", Float) = 1
        _LightRampValues5("Light Ramp Values [5]", Float) = 1
		_LightRampValues6("Light Ramp Values [6]", Float) = 1
		_LightRampValues7("Light Ramp Values [7]", Float) = 1
        
        _DotSize("Dot Size", Float) = 0.1
        _DotRotation("Dot Rotation", Float) = 0.125

		_HatchingWidth("Hatching Width", Float) = 0.5
		_HatchingOffsetPerLayer("Hatching Offset Per Layer", Float) = 0.001
		_HatchingRotation("Hatching Rotation", Float) = 0.125
		_HatchingCross("Hatching Cross", Float) = 1
        _HatchingFrequency("Hatching Frequency", Float) = 0.5
        
        _AccentLightWeight("Accent Lighting Weight", Range(0.0, 1.0)) = 0.75
        _AccentLightWidth("Accent Lighting Width", Range(0.0, 1.0)) = 0.25
        
        _StepSmoothness("Accent Light Smoothness", Range(0.0, 1.0)) = 0.1

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossMap("Specular", 2D) = "white" {}

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}
        
        _HeightMapDepth("Height Map Depth", Float) = 0.25
        _HeightMapFidelity("Height Map Fidelity", Range(0.0, 1.0)) = 0.5
        _HeightMap("Height", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}
        
        _RenderMode("Render Mode", Float) = 0

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0

        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
    }

    SubShader
    {
        // With SRP we introduce a new "RenderPipeline" tag in Subshader. This allows to create shaders
        // that can match multiple render pipelines. If a RenderPipeline tag is not set it will match
        // any render pipeline. In case you want your subshader to only run in LWRP set the tag to
        // "UniversalRenderPipeline"
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
        LOD 300

        Pass
        {
            // "Lightmode" tag must be "UniversalForward" or not be defined in order for
            // to render objects.
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            // unused shader_feature variants are stripped from build automatically
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _HEIGHTMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _OCCLUSIONMAP

            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_OFF
            #pragma shader_feature _DOTS_MODE
			#pragma shader_feature _HATCHING_MODE
            #pragma shader_feature _WORLD_BEND

            // -------------------------------------
            // Universal Render Pipeline keywords
            // When doing custom shaders you most often want to copy and past these #pragmas
            // These multi_compile variants are stripped from the build depending on:
            // 1) Settings in the LWRP Asset assigned in the GraphicsSettings at build time
            // e.g If you disable AdditionalLights in the asset then all _ADDITIONA_LIGHTS variants
            // will be stripped from build
            // 2) Invalid combinations are stripped. e.g variants with _MAIN_LIGHT_SHADOWS_CASCADE
            // but not _MAIN_LIGHT_SHADOWS are invalid and therefore stripped.
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex CellPassVertex
            #pragma fragment CellPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "./CellShading.hlsl"

            struct Attributes {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float2 lightmapUV   : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings {
                float2 uv                       : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
            
                float4 positionWSFF             : TEXCOORD2;
            
                float3 normalWS                 : TEXCOORD3;
            #if defined(_NORMALMAP) || defined(_HEIGHTMAP)
                float3 tangentWS                : TEXCOORD4;    // xyz: tangent, w: sign
                float3 bitangentWS              : TEXCOORD8;
            #endif
            
                float3 viewDirWS                : TEXCOORD5;
            
                float4 shadowCoord              : TEXCOORD6;
            
                float4 positionCS               : SV_POSITION;

                #if defined(_DOTS_MODE) || defined(_HATCHING_MODE)
                float4 positionSP               : TEXCOORD7;
                #endif
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform float3 _BendAmount;

            Varyings CellPassVertex(Attributes input) {
                Varyings output = (Varyings)0;
            
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                
                // normalWS and tangentWS already normalize.
                // this is required to avoid skewing the direction during interpolation
                // also required for per-vertex lighting and SH evaluation
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                float3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
            
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
            
                // already normalized from normal transform to WS.
                output.normalWS = SafeNormalize(normalInput.normalWS);
                output.viewDirWS = viewDirWS;
                #if defined(_NORMALMAP) || defined(_HEIGHTMAP)
                    output.tangentWS = SafeNormalize(normalInput.tangentWS.xyz);
                    output.bitangentWS = SafeNormalize(normalInput.bitangentWS.xyz);
                #endif
            
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
            
                output.positionWSFF = float4(vertexInput.positionWS.xyz, fogFactor);
            
                output.shadowCoord = GetShadowCoord(vertexInput);

                #ifdef _WORLD_BEND
                float2 dist = viewDirWS.xz * viewDirWS.xz * _BendAmount.xz;
                float bend = dist.x + dist.y;
                #else
                float bend = 0;
                #endif
                
                output.positionCS = TransformWorldToHClip(vertexInput.positionWS + float3(0, -1, 0) * bend);

                #if defined(_DOTS_MODE) || defined(_HATCHING_MODE)
                output.positionSP = ComputeScreenPos(output.positionCS);
                output.positionSP.z = length(GetCameraPositionWS() - TransformObjectToWorld(float4(0, 0, 0, 1)));
                #endif
            
                return output;
            }

            half4 CellPassFragment(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.uv;
                float3 positionWS = input.positionWSFF.xyz;
                float3 viewWS = SafeNormalize(input.viewDirWS);

                #ifdef _HEIGHTMAP
                    // utilize inverse of tbn so that we have a world -> tangent space matrix
                    float3 viewTS = float3(input.tangentWS.x, input.bitangentWS.x, input.normalWS.x) * viewWS.x;
                    viewTS += float3(input.tangentWS.y, input.bitangentWS.y, input.normalWS.y) * viewWS.y;
                    viewTS += float3(input.tangentWS.z, input.bitangentWS.z, input.normalWS.z) * viewWS.z;
                
                    float layers = 1.0 - saturate(dot(input.normalWS, viewWS));
                    uv = ParallaxMapping(
                        uv, viewTS, lerp(6.0, 128.0, layers * _HeightMapFidelity),
                        TEXTURE2D_ARGS(_HeightMap, sampler_HeightMap)
                    );
                #endif

                #ifdef _NORMALMAP
                    float3x3 tbn = float3x3(
                        input.tangentWS,
                        input.bitangentWS,
                        input.normalWS
                    );

                    float3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
                    float3 normalWS = TransformTangentToWorld(normalTS, tbn);
                #else
                    float3 normalWS = input.normalWS;
                #endif

                float3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
                float occlusion = SampleOcclusion(uv);
                float4 specularSample = SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv);
                float3 specular = specularSample.rgb * _SpecColor.rgb;
                float smoothness = specularSample.a * _Smoothness;
                
                normalWS = SafeNormalize(normalWS);
                float3 cam = SafeNormalize(-mul(UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V))[2].xyz));

                #ifdef LIGHTMAP_ON
                    half3 bakedGI = SampleLightmap(input.uvLM, normalWS);
                #else
                    half3 bakedGI = SampleSH(normalWS);
                #endif

                #ifdef _MAIN_LIGHT_SHADOWS
                    Light mainLight = GetMainLight(input.shadowCoord);
                #else
                    Light mainLight = GetMainLight();
                #endif
                
                #ifdef _ADDITIONAL_LIGHTS
                    int lightCount = 1 + GetAdditionalLightsCount();
                #else
                    int lightCount = 1;
                #endif
                
                float giAtten = length(bakedGI) / (1.73205080757 * lightCount);
                
                float diffuse = 0;
                float back = 0;
                float side = 0;
                float spec = 0;
                
                float atten = (mainLight.shadowAttenuation * mainLight.distanceAttenuation);
                float intensity = length(mainLight.color) / 1.73205080757;

                // main lighting
                CellFullLighting(
                    normalWS, mainLight.direction, intensity,
                    viewWS, occlusion, atten, smoothness,
                    diffuse, spec
                );
                
                float3 lightTint = mainLight.color * diffuse;

                // additional lights loop
                #ifdef _ADDITIONAL_LIGHTS
                    int additionalLightsCount = lightCount - 1;
                    for (int i = 0; i < additionalLightsCount; ++i) {
                        Light light = GetAdditionalLight(i, positionWS);
                        
                        atten = (light.shadowAttenuation * light.distanceAttenuation);
                        intensity = length(light.color) / 1.73205080757;
                        float d = 0;
                        CellFullLighting(
                            normalWS, light.direction, intensity,
                            viewWS, occlusion, atten, smoothness,
                            d, spec
                        );
                        
                        lightTint += light.color * d;
                        diffuse += d;
                    }
                #endif

                #ifdef _DOTS_MODE
                float2 screenPos = input.positionSP.xy / input.positionSP.w;
                screenPos *= _ScreenParams.xy / min(_ScreenParams.x, _ScreenParams.y);
                
                half4 out_col = FinalDotsCalculation(
                    normalWS, viewWS, uv, screenPos,
                    diffuse, spec, smoothness,
                    input.positionWSFF.w, specular, lightTint, emission,
                    TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)
                );
                #endif

				#ifdef _HATCHING_MODE
				float2 screenPos = input.positionSP.xy / input.positionSP.w;
                screenPos *= _ScreenParams.xy / min(_ScreenParams.x, _ScreenParams.y);
                
                half4 out_col = FinalCrossHatchCalculation(
                    normalWS, viewWS, uv, screenPos, input.positionSP.z,
                    diffuse, spec, smoothness,
                    input.positionWSFF.w, specular, lightTint, emission,
                    TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)
                );
				#else

                half4 out_col = FinalBandsCalculation(
                    normalWS, viewWS, uv,
                    diffuse, spec, smoothness,
                    input.positionWSFF.w, specular, lightTint, emission,
                    TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)
                );

                #endif

                return out_col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "./CellShading.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "./CellShading.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex UniversalVertexMeta
            #pragma fragment MetaPassFragment

            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma shader_feature _SPECGLOSSMAP

            #include "./CellShading.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv0          : TEXCOORD0;
                float2 uv1          : TEXCOORD1;
                float2 uv2          : TEXCOORD2;
            #ifdef _TANGENT_TO_WORLD
                float4 tangentOS     : TANGENT;
            #endif
            };
            
            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };
            
            Varyings UniversalVertexMeta(Attributes input)
            {
                Varyings output;
                output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2,
                    unity_LightmapST, unity_DynamicLightmapST);
                output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
                return output;
            }
            
            half4 MetaPassFragment(Varyings input) : SV_Target {
                float3 diffuse = CellAlbedo(input.uv, _BaseColor, half3(1, 1, 1), 0, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).rgb;
                
                float4 specularSample = SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, input.uv);
                float3 specular = specularSample.rgb * _SpecColor.rgb;
                float smoothness = specularSample.a * _Smoothness;
                
                MetaInput metaInput;
                metaInput.Albedo = diffuse + specular * saturate(1.0 - smoothness) * 0.5;
                metaInput.SpecularColor = specular;
                metaInput.Emission = SampleEmission(input.uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
            
                return MetaFragment(metaInput);
            }

            ENDHLSL
        }
    }

    // Uses a custom shader GUI to display settings. Re-use the same from Lit shader as they have the
    // same properties.
    CustomEditor "CellShading.Editor.CellShading"
}
