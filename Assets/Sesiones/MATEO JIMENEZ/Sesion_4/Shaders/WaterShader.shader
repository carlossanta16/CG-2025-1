Shader "Custom/WaterShaderURP"
{
    Properties
    {
        _Color ("Color", Color) = (0.0, 0.5, 0.8, 0.8)
        _DepthColor ("Depth Color", Color) = (0.0, 0.1, 0.3, 1.0)
        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        [NoScaleOffset] _NoiseTexture ("Noise Texture", 2D) = "white" {}
        [NoScaleOffset] _FoamTexture ("Foam Texture", 2D) = "white" {}
        [NoScaleOffset] _Cubemap ("Cubemap", CUBE) = "" {}
        
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.2
        _WaveFrequency ("Wave Frequency", Range(0, 10)) = 2.0
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        
        _RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.2
        _ReflectionStrength ("Reflection Strength", Range(0, 1)) = 0.5
        _Glossiness ("Smoothness", Range(0, 1)) = 0.8
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 5.0
        
        _DepthDistance ("Depth Distance", Range(0, 10)) = 2.0
        _DepthFalloff ("Depth Falloff", Range(0, 10)) = 2.0
        
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _IntersectionFoamDepth ("Intersection Foam Depth", Range(0, 2)) = 0.5
        _FoamCutoff ("Foam Cutoff", Range(0, 1)) = 0.8
        _FoamScale ("Foam Scale", Range(0, 10)) = 5.0
        _WaveFoamCutoff ("Wave Foam Cutoff", Range(0, 1)) = 0.9
    }
    
    SubShader
    {
        Tags { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }
        
        LOD 200
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
        
        CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _DepthColor;
            float4 _FoamColor;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _RefractionStrength;
            float _ReflectionStrength;
            float _Glossiness;
            float _FresnelPower;
            float _DepthDistance;
            float _DepthFalloff;
            float _IntersectionFoamDepth;
            float _FoamCutoff;
            float _FoamScale;
            float _WaveFoamCutoff;
        CBUFFER_END
        
        TEXTURE2D(_NormalMap);        SAMPLER(sampler_NormalMap);
        TEXTURE2D(_NoiseTexture);     SAMPLER(sampler_NoiseTexture);
        TEXTURE2D(_FoamTexture);      SAMPLER(sampler_FoamTexture);
        TEXTURECUBE(_Cubemap);        SAMPLER(sampler_Cubemap);
        
        // Wave and displacement helper functions
        float sineWave(float2 position, float2 direction, float frequency, float speed, float time, float amplitude)
        {
            return sin((dot(position, direction) * frequency + time * speed)) * amplitude;
        }
        
        float calculateWaveHeight(float3 position, float time)
        {
            // Combine multiple waves for more complexity
            float wave1 = sineWave(position.xz, normalize(float2(1, 0.5)), _WaveFrequency, _WaveSpeed, time, 1.0);
            float wave2 = sineWave(position.xz, normalize(float2(0.7, 1)), _WaveFrequency * 0.8, _WaveSpeed * 1.2, time, 0.8);
            float wave3 = sineWave(position.xz, normalize(float2(-0.3, 0.9)), _WaveFrequency * 1.3, _WaveSpeed * 0.8, time, 0.6);
            
            return (wave1 + wave2 + wave3) * _WaveAmplitude;
        }
        
        float calculateWaveFoam(float3 position, float time)
        {
            // Similar to height calculation but with slight variations for foam
            float wave1 = sineWave(position.xz, normalize(float2(0.9, 0.6)), _WaveFrequency * 1.1, _WaveSpeed * 1.1, time, 1.0);
            float wave2 = sineWave(position.xz, normalize(float2(0.8, 1.0)), _WaveFrequency * 0.9, _WaveSpeed * 0.9, time, 0.7);
            
            // Normalize to 0-1 range
            return (wave1 + wave2) * 0.5 + 0.5;
        }
        
        float3 calculateWaveNormal(float3 position, float time, float epsilon)
        {
            // Calculate normals using finite differences
            float height = calculateWaveHeight(position, time);
            float heightX = calculateWaveHeight(position + float3(epsilon, 0, 0), time);
            float heightZ = calculateWaveHeight(position + float3(0, 0, epsilon), time);
            
            // Calculate the partial derivatives
            float dhdx = (heightX - height) / epsilon;
            float dhdz = (heightZ - height) / epsilon;
            
            // Construct the normal vector
            return normalize(float3(-dhdx, 1.0, -dhdz));
        }
        ENDHLSL
        
        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
            };
            
            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
                float3 positionWS   : TEXCOORD1;
                float3 normalWS     : TEXCOORD2;
                float3 tangentWS    : TEXCOORD3;
                float3 bitangentWS  : TEXCOORD4;
                float4 screenPos    : TEXCOORD5;
                float eyeDepth      : TEXCOORD6;
                float3 viewDirWS    : TEXCOORD7;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Calculate time for wave animation
                float time = _Time.y;
                
                // Apply vertex displacement for waves
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float waveHeight = calculateWaveHeight(positionWS, time);
                input.positionOS.y += waveHeight;
                
                // Recalculate positionWS after displacement
                positionWS = TransformObjectToWorld(input.positionOS.xyz);
                
                // Calculate normal in world space
                float3 normalWS = calculateWaveNormal(positionWS, time, 0.01);
                
                // Build output struct
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.positionWS = positionWS;
                output.normalWS = normalWS;
                
                // Calculate tangent and bitangent for normal mapping
                float3 tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);
                float3 bitangentWS = cross(normalWS, tangentWS) * input.tangentOS.w;
                output.tangentWS = tangentWS;
                output.bitangentWS = bitangentWS;
                
                // Calculate screen position for refraction
                output.screenPos = ComputeScreenPos(output.positionHCS);
                output.eyeDepth = -TransformWorldToView(positionWS).z;
                
                // Calculate view direction
                output.viewDirWS = GetWorldSpaceViewDir(positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y;
                
                // ---------- 1. Sample normal maps ----------
                float2 normalUV1 = input.uv + float2(time * 0.03, time * 0.05);
                float2 normalUV2 = input.uv * 1.5 + float2(-time * 0.04, time * 0.02);
                float3 normalMap1 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, normalUV1));
                float3 normalMap2 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, normalUV2));
                float3 normalMap = normalize(normalMap1 + normalMap2 * 0.5);
                
                // Create TBN matrix for normal mapping
                float3x3 TBN = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(normalMap, TBN));
                
                // ---------- 2. Calculate water depth for fog ----------
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                #if UNITY_REVERSED_Z
                    float sceneDepth = SampleSceneDepth(screenUV);
                #else
                    float sceneDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(screenUV));
                #endif
                
                float linearSceneDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
                float surfaceDepth = input.eyeDepth;
                float depthDifference = saturate((linearSceneDepth - surfaceDepth) / _DepthDistance);
                
                // ---------- 3. Screen space refraction ----------
                // Apply normal-based distortion
                float2 refractionOffset = normalMap.xy * _RefractionStrength;
                float2 refractionUV = screenUV + refractionOffset;
                half4 refractedColor = half4(SampleSceneColor(refractionUV), 1.0);
                
                // ---------- 4. Depth-based color lerp (depth fog) ----------
                half4 waterColor = lerp(_Color, _DepthColor, pow(depthDifference, _DepthFalloff));
                half4 baseColor = lerp(refractedColor, waterColor, waterColor.a);
                
                // ---------- 5. Cubemap reflection ----------
                float3 viewDirWS = normalize(input.viewDirWS);
                float3 viewReflection = reflect(-viewDirWS, normalWS);
                half4 reflectionColor = SAMPLE_TEXTURECUBE(_Cubemap, sampler_Cubemap, viewReflection);
                
                // Fresnel effect for reflection strength
                float fresnel = pow(1.0 - saturate(dot(viewDirWS, normalWS)), _FresnelPower);
                half4 reflectedColor = lerp(baseColor, reflectionColor, fresnel * _ReflectionStrength);
                
                // ---------- 6. Intersection foam ----------
                float foamDepthDifference = saturate((linearSceneDepth - surfaceDepth) / _IntersectionFoamDepth);
                float foamMask = 1.0 - foamDepthDifference;
                
                float2 foamUV = input.uv * _FoamScale + time * 0.1;
                float4 foamNoise = SAMPLE_TEXTURE2D(_FoamTexture, sampler_FoamTexture, foamUV);
                float intersectionFoam = saturate(foamNoise.r * foamMask);
                intersectionFoam = (intersectionFoam > _FoamCutoff) ? 1.0 : 0.0;
                
                // ---------- 7. Wave foam (only visible on wave peaks) ----------
                float waveHeight = calculateWaveFoam(input.positionWS, time);
                float waveFoam = (waveHeight > _WaveFoamCutoff) ? 1.0 : 0.0;
                
                // Combine both foam types
                float totalFoam = saturate(intersectionFoam + waveFoam);
                half4 colorWithFoam = lerp(reflectedColor, _FoamColor, totalFoam);
                
                // ---------- 8. Specular lighting (Blinn-Phong) ----------
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 halfVector = normalize(lightDir + viewDirWS);
                float specularNdotH = max(0, dot(normalWS, halfVector));
                float specularIntensity = pow(specularNdotH, _Glossiness * 100);
                
                // Add specular highlight
                float3 specularHighlight = mainLight.color * specularIntensity * _Glossiness;
                float3 finalColor = colorWithFoam.rgb + specularHighlight;
                
                // Final alpha
                float alpha = lerp(_Color.a, 1.0, totalFoam);
                
                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
        
        // Shadow casting support (optional)
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Calculate time for wave animation
                float time = _Time.y;
                
                // Apply vertex displacement for waves
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float waveHeight = calculateWaveHeight(positionWS, time);
                input.positionOS.y += waveHeight;
                
                // Transform to clip space
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        
        // Depth Only Pass (optional - for depth prepass)
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Calculate time for wave animation
                float time = _Time.y;
                
                // Apply vertex displacement for waves
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float waveHeight = calculateWaveHeight(positionWS, time);
                input.positionOS.y += waveHeight;
                
                // Transform to clip space
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}