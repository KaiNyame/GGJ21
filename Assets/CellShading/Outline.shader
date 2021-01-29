Shader "Outline/Outline Material"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline Width", Range (.002, 0.03)) = .005
    	[Toggle(_COLOR_NORMALS)] _ColorNormals ("Normals Baked Into Colors", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
        	Cull Front
			Name "OUTLINE"
			
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

            #pragma shader_feature _COLOR_NORMALS
            #pragma shader_feature _WORLD_BEND
			
            CBUFFER_START(UnityPerMaterial)
            float _Outline;
            float _ColorNormals;
            float4 _OutlineColor;
            CBUFFER_END
			
            struct Attributes 
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            	float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
        
            struct Varyings 
            {
                float4 positionCS : SV_POSITION;
                half fogCoord : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform float3 _BendAmount;
            
            Varyings vert(Attributes input) 
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

				#ifdef _COLOR_NORMALS
            	float3 n = input.color.rgb * 2.0 - 1.0;
            	#else
            	float3 n = input.normalOS;
            	#endif

            	#ifdef _WORLD_BEND
            	float3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                float2 dist = viewDirWS.xz * viewDirWS.xz * _BendAmount.xz;
            	float4 posCS = TransformWorldToHClip(vertexInput.positionWS + float3(0, -1, 0) * (dist.x + dist.y));
                #else
                float4 posCS = vertexInput.positionCS;
                #endif
            	
				float3 normalCS = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, input.normalOS));
				float2 normal = SafeNormalize(float3(normalCS.xy, 0)).xy * posCS.w;
            	
            	float2 screen_texel = _ScreenParams.zw - 1.0;
				float2 offset = normal * _Outline / (_ScreenParams.xy * max(screen_texel.x, screen_texel.y)) * 2;
            	
                output.positionCS = posCS + float4(offset, 0, 0);
            	
                output.color = _OutlineColor;
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                return output;
            }
			
			half4 frag(Varyings i) : SV_Target
			{
				i.color.rgb = MixFog(i.color.rgb, i.fogCoord);
				return i.color;
			}
            ENDHLSL
        }
    }
}
