Shader "ConnectFourMultiplayer/VolumetricFog"
{
    Properties
    {
        _Color("Color" , Color) = (1, 1, 1, 1)
        _MaxDistance("Max distance", float) = 100     
        _StepSize("Step size", Range(0.1, 20)) = 1
        _DistanceMultiplier("Distance multiplier", Range(0,10)) = 1
        _NoiseOffset("Noise offset", float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 _Color;
            float _MaxDistance;
            float _DistanceMultiplier;
            float _StepSize;
            float _NoiseOffset;

            float get_density()
            {
                return _DistanceMultiplier;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float rayDir = normalize(viewDir);

                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
                float distanceLimit = min(viewLength, _MaxDistance);
                float distanceTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
                float transmittance = 1;

                while (distanceTravelled < distanceLimit)
                {
                    float density = get_density();
                    if(density > 0)
                    {
                        transmittance *= exp(-density * _StepSize);
                    }

                    distanceTravelled += _StepSize;
                }

                return lerp(col, _Color, 1.0 - saturate(transmittance));
            }
            ENDHLSL
        }
    }
}
