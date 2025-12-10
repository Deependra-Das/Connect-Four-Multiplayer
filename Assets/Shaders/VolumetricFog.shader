Shader "ConnectFourMultiplayer/VolumetricFog"
{
    Properties
    {
        _MaxDistance("Max distance", float) = 100     
        _StepSize("Step size", Range(0.1, 20)) = 1
        _DistanceMultiplier("Distance multiplier", Range(0,10)) = 1
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

               float _MaxDistance;
               float _DistanceMultiplier;
               float _StepSize;

            float get_density()
            {
                return _DistanceMultiplier;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float rayDir = normalize(viewDir);

                float distanceLimit = min(viewLength, _MaxDistance);
                float distanceTravelled = 0;
                float transmittance = 0;

                while (distanceTravelled < distanceLimit)
                {
                    float density = get_density();
                    if(density > 0)
                    {
                        transmittance += density * _StepSize;
                    }

                    distanceTravelled += _StepSize;
                }

                return transmittance;
            }
            ENDHLSL
        }
    }
}
