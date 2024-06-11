Shader "Class/UI_ScrollDistort_Modified"
    {
        Properties
        {
            [HDR]_HDR("HDR", Color) = (1, 0.5415801, 0, 0)
            _MainOffset("MainOffset", Vector) = (0, 0, 0, 0)
            _MainTiling("MainTiling", Vector) = (1, 1, 0, 0)
            [ToggleUI]_StaticOffset("StaticOffset", Float) = 0
            [NoScaleOffset]_DistortTex("DistortTex", 2D) = "white" {}
            _Distort("Distort", Float) = 0.55
            _DistortOffset("DistortOffset", Vector) = (0, 0, 0, 0)
            _DistortTiling("DistortTiling", Vector) = (1, 1, 0, 0)
            _MinMax("MinMax", Vector) = (0, 1, 0, 0)
            _Power("Power", Float) = 1
            [NoScaleOffset]_MaskTex("MaskTex", 2D) = "white" {}
            [ToggleUI]_Mask("Mask", Float) = 0
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

            _StencilComp ("Stencil Comparison", Float) = 8
            _Stencil ("Stencil ID", Float) = 0
            _StencilOp ("Stencil Operation", Float) = 0
            _StencilWriteMask ("Stencil Write Mask", Float) = 255
            _StencilReadMask ("Stencil Read Mask", Float) = 255
            _ColorMask ("Color Mask", Float) = 15
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Transparent"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Transparent"
                "ShaderGraphShader"="true"
                "ShaderGraphTargetId"=""
            }

            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            ColorMask [_ColorMask]


            Pass
            {
                Name "Sprite Unlit"
                Tags
                {
                    "LightMode" = "Universal2D"
                }
            
                // Render State
                Cull Off
                Blend SrcAlpha One, One One
                ZTest [unity_GUIZTestMode]
                ZWrite Off
            
                // Debug
                // <None>
            
                // --------------------------------------------------
                // Pass
            
                HLSLPROGRAM
            
                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
            
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
            
                // Keywords
                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                // GraphKeywords: <None>
            
                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define _BLENDMODE_ADD 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_COLOR
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITEUNLIT
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
                // Includes
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
            
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
                // --------------------------------------------------
                // Structs and Packing
            
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float4 texCoord0;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                     float3 TimeParameters;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                     float4 color : INTERP1;
                     float3 positionWS : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    output.color.xyzw = input.color;
                    output.positionWS.xyz = input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    output.color = input.color.xyzw;
                    output.positionWS = input.positionWS.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
                // --------------------------------------------------
                // Graph
            
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _DistortTex_TexelSize;
                float2 _DistortTiling;
                float _Mask;
                float _Distort;
                float _Power;
                float4 _HDR;
                float2 _MainTiling;
                float2 _DistortOffset;
                float2 _MainOffset;
                float2 _MinMax;
                float4 _MaskTex_TexelSize;
                float _StaticOffset;
                CBUFFER_END
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                float4 _MainTex_TexelSize;
                TEXTURE2D(_DistortTex);
                SAMPLER(sampler_DistortTex);
                TEXTURE2D(_MaskTex);
                SAMPLER(sampler_MaskTex);
            
                // Graph Includes
                // GraphIncludes: <None>
            
                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif
            
                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif
            
                // Graph Functions
                
                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Branch_float2(float Predicate, float2 True, float2 False, out float2 Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                {
                    Out = UV * Tiling + Offset;
                }
                
                void Unity_Add_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }
                
                void Unity_Power_float(float A, float B, out float Out)
                {
                    Out = pow(A, B);
                }
                
                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Saturate_float4(float4 In, out float4 Out)
                {
                    Out = saturate(In);
                }
                
                void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
                {
                    Out = Predicate ? True : False;
                }
            
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
            
                #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Property_1083bf5988a14e99a816880fe20f59c1_Out_0 = _Mask;
                    float4 _Property_2a7bc19c81b347b889230d21744aec4e_Out_0 = IsGammaSpace() ? LinearToSRGB(_HDR) : _HDR;
                    float2 _Property_cc47aa35420d4d0fa5551bb32f6af68b_Out_0 = _MinMax;
                    float _Split_416551e7c42b47d7bd4bc436e535c963_R_1 = _Property_cc47aa35420d4d0fa5551bb32f6af68b_Out_0[0];
                    float _Split_416551e7c42b47d7bd4bc436e535c963_G_2 = _Property_cc47aa35420d4d0fa5551bb32f6af68b_Out_0[1];
                    float _Split_416551e7c42b47d7bd4bc436e535c963_B_3 = 0;
                    float _Split_416551e7c42b47d7bd4bc436e535c963_A_4 = 0;
                    UnityTexture2D _Property_e477ef47724940d883bf7c40fee8f91b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                    float2 _Property_2f600a9f67a74f8fa0326bf87f6184d1_Out_0 = _MainTiling;
                    float _Property_bf885ac15df54baf97ebba18184066ad_Out_0 = _StaticOffset;
                    float2 _Property_77537d7217a04effbce77ed2d29c603b_Out_0 = _MainOffset;
                    float2 _Multiply_7f0a15fc085b452094a1437ea49f25a0_Out_2;
                    Unity_Multiply_float2_float2(_Property_77537d7217a04effbce77ed2d29c603b_Out_0, (IN.TimeParameters.x.xx), _Multiply_7f0a15fc085b452094a1437ea49f25a0_Out_2);
                    float2 _Branch_3df6a2bc3ceb41c3b1f8e3ef406caf4b_Out_3;
                    Unity_Branch_float2(_Property_bf885ac15df54baf97ebba18184066ad_Out_0, _Property_77537d7217a04effbce77ed2d29c603b_Out_0, _Multiply_7f0a15fc085b452094a1437ea49f25a0_Out_2, _Branch_3df6a2bc3ceb41c3b1f8e3ef406caf4b_Out_3);
                    float2 _TilingAndOffset_cb43b0e21cbd4c0cb62d5d407d232ee9_Out_3;
                    Unity_TilingAndOffset_float(IN.uv0.xy, _Property_2f600a9f67a74f8fa0326bf87f6184d1_Out_0, _Branch_3df6a2bc3ceb41c3b1f8e3ef406caf4b_Out_3, _TilingAndOffset_cb43b0e21cbd4c0cb62d5d407d232ee9_Out_3);
                    UnityTexture2D _Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0 = UnityBuildTexture2DStructNoScale(_DistortTex);
                    float2 _Property_5cb56026df89482f9dd50d0f86f9a4b0_Out_0 = _DistortTiling;
                    float2 _Property_04c1ac0ccb444fbf8c03b224bbbb4d29_Out_0 = _DistortOffset;
                    float2 _Multiply_7becf085a5d243eca76bdf37a02e12e6_Out_2;
                    Unity_Multiply_float2_float2(_Property_04c1ac0ccb444fbf8c03b224bbbb4d29_Out_0, (IN.TimeParameters.x.xx), _Multiply_7becf085a5d243eca76bdf37a02e12e6_Out_2);
                    float2 _TilingAndOffset_ac94c280ba98466c9da94c75df3c1cf1_Out_3;
                    Unity_TilingAndOffset_float(IN.uv0.xy, _Property_5cb56026df89482f9dd50d0f86f9a4b0_Out_0, _Multiply_7becf085a5d243eca76bdf37a02e12e6_Out_2, _TilingAndOffset_ac94c280ba98466c9da94c75df3c1cf1_Out_3);
                    float4 _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0.tex, _Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0.samplerstate, _Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0.GetTransformedUV(_TilingAndOffset_ac94c280ba98466c9da94c75df3c1cf1_Out_3));
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_R_4 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.r;
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_G_5 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.g;
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_B_6 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.b;
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_A_7 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.a;
                    float2 _Vector2_44cfddc4d47949c2b7c3fb5a266b368e_Out_0 = float2(_SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_R_4, _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_G_5);
                    float _Property_b2652f4c92094484963087b5944a70ca_Out_0 = _Distort;
                    float2 _Multiply_65aaa81e0e92400182c41c2fa357113c_Out_2;
                    Unity_Multiply_float2_float2(_Vector2_44cfddc4d47949c2b7c3fb5a266b368e_Out_0, (_Property_b2652f4c92094484963087b5944a70ca_Out_0.xx), _Multiply_65aaa81e0e92400182c41c2fa357113c_Out_2);
                    float2 _Add_048f26e03f754344805d932eff6f795f_Out_2;
                    Unity_Add_float2(_TilingAndOffset_cb43b0e21cbd4c0cb62d5d407d232ee9_Out_3, _Multiply_65aaa81e0e92400182c41c2fa357113c_Out_2, _Add_048f26e03f754344805d932eff6f795f_Out_2);
                    float4 _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e477ef47724940d883bf7c40fee8f91b_Out_0.tex, _Property_e477ef47724940d883bf7c40fee8f91b_Out_0.samplerstate, _Property_e477ef47724940d883bf7c40fee8f91b_Out_0.GetTransformedUV(_Add_048f26e03f754344805d932eff6f795f_Out_2));
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_R_4 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.r;
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_G_5 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.g;
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_B_6 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.b;
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_A_7 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.a;
                    float _Smoothstep_b424630841a54791b2a554abf052015e_Out_3;
                    Unity_Smoothstep_float(_Split_416551e7c42b47d7bd4bc436e535c963_R_1, _Split_416551e7c42b47d7bd4bc436e535c963_G_2, _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_R_4, _Smoothstep_b424630841a54791b2a554abf052015e_Out_3);
                    float _Property_2ece1a2fcaa347ecba9d2346a5d4655b_Out_0 = _Power;
                    float _Power_e166e5c0e67d480bbb8903b8fb04ac08_Out_2;
                    Unity_Power_float(_Smoothstep_b424630841a54791b2a554abf052015e_Out_3, _Property_2ece1a2fcaa347ecba9d2346a5d4655b_Out_0, _Power_e166e5c0e67d480bbb8903b8fb04ac08_Out_2);
                    float4 _Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2;
                    Unity_Multiply_float4_float4(_Property_2a7bc19c81b347b889230d21744aec4e_Out_0, (_Power_e166e5c0e67d480bbb8903b8fb04ac08_Out_2.xxxx), _Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2);
                    UnityTexture2D _Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
                    float4 _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0.tex, _Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0.samplerstate, _Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0.GetTransformedUV(IN.uv0.xy));
                    float _SampleTexture2D_873db9df805946b884390669ba634753_R_4 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.r;
                    float _SampleTexture2D_873db9df805946b884390669ba634753_G_5 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.g;
                    float _SampleTexture2D_873db9df805946b884390669ba634753_B_6 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.b;
                    float _SampleTexture2D_873db9df805946b884390669ba634753_A_7 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.a;
                    float4 _Multiply_f25fd46e138f4dfb8f796c1b5a44ec70_Out_2;
                    Unity_Multiply_float4_float4(_Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2, (_SampleTexture2D_873db9df805946b884390669ba634753_R_4.xxxx), _Multiply_f25fd46e138f4dfb8f796c1b5a44ec70_Out_2);
                    float4 _Saturate_63908d978ae0497ca92a1453a374852d_Out_1;
                    Unity_Saturate_float4(_Multiply_f25fd46e138f4dfb8f796c1b5a44ec70_Out_2, _Saturate_63908d978ae0497ca92a1453a374852d_Out_1);
                    float4 _Branch_ca00cede9c6845f8acaff6ba576c90f8_Out_3;
                    Unity_Branch_float4(_Property_1083bf5988a14e99a816880fe20f59c1_Out_0, _Saturate_63908d978ae0497ca92a1453a374852d_Out_1, _Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2, _Branch_ca00cede9c6845f8acaff6ba576c90f8_Out_3);
                    surface.BaseColor = (_Branch_ca00cede9c6845f8acaff6ba576c90f8_Out_3.xyz);
                    surface.Alpha = 1;
                    return surface;
                }
            
                // --------------------------------------------------
                // Build Graph Inputs
            
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                    
                
                
                
                
                
                    output.uv0 =                                        input.texCoord0;
                    output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
            
                // --------------------------------------------------
                // Main
            
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
            
                ENDHLSL
            }


            Pass
            {
                Name "Sprite Unlit"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }
            
                // Render State
                Cull Off
                Blend SrcAlpha One, One One
                ZTest LEqual
                ZWrite Off
            
                // Debug
                // <None>
            
                // --------------------------------------------------
                // Pass
            
                HLSLPROGRAM
            
                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
            
                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>
            
                // Keywords
                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                // GraphKeywords: <None>
            
                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define _BLENDMODE_ADD 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_COLOR
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITEFORWARD
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
                // Includes
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
            
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
                // --------------------------------------------------
                // Structs and Packing
            
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float4 texCoord0;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                     float3 TimeParameters;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                     float4 color : INTERP1;
                     float3 positionWS : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    output.color.xyzw = input.color;
                    output.positionWS.xyz = input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    output.color = input.color.xyzw;
                    output.positionWS = input.positionWS.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
                // --------------------------------------------------
                // Graph
            
                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _DistortTex_TexelSize;
                float2 _DistortTiling;
                float _Mask;
                float _Distort;
                float _Power;
                float4 _HDR;
                float2 _MainTiling;
                float2 _DistortOffset;
                float2 _MainOffset;
                float2 _MinMax;
                float4 _MaskTex_TexelSize;
                float _StaticOffset;
                CBUFFER_END
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                float4 _MainTex_TexelSize;
                TEXTURE2D(_DistortTex);
                SAMPLER(sampler_DistortTex);
                TEXTURE2D(_MaskTex);
                SAMPLER(sampler_MaskTex);
            
                // Graph Includes
                // GraphIncludes: <None>
            
                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif
            
                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif
            
                // Graph Functions
                
                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Branch_float2(float Predicate, float2 True, float2 False, out float2 Out)
                {
                    Out = Predicate ? True : False;
                }
                
                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                {
                    Out = UV * Tiling + Offset;
                }
                
                void Unity_Add_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }
                
                void Unity_Power_float(float A, float B, out float Out)
                {
                    Out = pow(A, B);
                }
                
                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Saturate_float4(float4 In, out float4 Out)
                {
                    Out = saturate(In);
                }
                
                void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
                {
                    Out = Predicate ? True : False;
                }
            
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
            
                #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Property_1083bf5988a14e99a816880fe20f59c1_Out_0 = _Mask;
                    float4 _Property_2a7bc19c81b347b889230d21744aec4e_Out_0 = IsGammaSpace() ? LinearToSRGB(_HDR) : _HDR;
                    float2 _Property_cc47aa35420d4d0fa5551bb32f6af68b_Out_0 = _MinMax;
                    float _Split_416551e7c42b47d7bd4bc436e535c963_R_1 = _Property_cc47aa35420d4d0fa5551bb32f6af68b_Out_0[0];
                    float _Split_416551e7c42b47d7bd4bc436e535c963_G_2 = _Property_cc47aa35420d4d0fa5551bb32f6af68b_Out_0[1];
                    float _Split_416551e7c42b47d7bd4bc436e535c963_B_3 = 0;
                    float _Split_416551e7c42b47d7bd4bc436e535c963_A_4 = 0;
                    UnityTexture2D _Property_e477ef47724940d883bf7c40fee8f91b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                    float2 _Property_2f600a9f67a74f8fa0326bf87f6184d1_Out_0 = _MainTiling;
                    float _Property_bf885ac15df54baf97ebba18184066ad_Out_0 = _StaticOffset;
                    float2 _Property_77537d7217a04effbce77ed2d29c603b_Out_0 = _MainOffset;
                    float2 _Multiply_7f0a15fc085b452094a1437ea49f25a0_Out_2;
                    Unity_Multiply_float2_float2(_Property_77537d7217a04effbce77ed2d29c603b_Out_0, (IN.TimeParameters.x.xx), _Multiply_7f0a15fc085b452094a1437ea49f25a0_Out_2);
                    float2 _Branch_3df6a2bc3ceb41c3b1f8e3ef406caf4b_Out_3;
                    Unity_Branch_float2(_Property_bf885ac15df54baf97ebba18184066ad_Out_0, _Property_77537d7217a04effbce77ed2d29c603b_Out_0, _Multiply_7f0a15fc085b452094a1437ea49f25a0_Out_2, _Branch_3df6a2bc3ceb41c3b1f8e3ef406caf4b_Out_3);
                    float2 _TilingAndOffset_cb43b0e21cbd4c0cb62d5d407d232ee9_Out_3;
                    Unity_TilingAndOffset_float(IN.uv0.xy, _Property_2f600a9f67a74f8fa0326bf87f6184d1_Out_0, _Branch_3df6a2bc3ceb41c3b1f8e3ef406caf4b_Out_3, _TilingAndOffset_cb43b0e21cbd4c0cb62d5d407d232ee9_Out_3);
                    UnityTexture2D _Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0 = UnityBuildTexture2DStructNoScale(_DistortTex);
                    float2 _Property_5cb56026df89482f9dd50d0f86f9a4b0_Out_0 = _DistortTiling;
                    float2 _Property_04c1ac0ccb444fbf8c03b224bbbb4d29_Out_0 = _DistortOffset;
                    float2 _Multiply_7becf085a5d243eca76bdf37a02e12e6_Out_2;
                    Unity_Multiply_float2_float2(_Property_04c1ac0ccb444fbf8c03b224bbbb4d29_Out_0, (IN.TimeParameters.x.xx), _Multiply_7becf085a5d243eca76bdf37a02e12e6_Out_2);
                    float2 _TilingAndOffset_ac94c280ba98466c9da94c75df3c1cf1_Out_3;
                    Unity_TilingAndOffset_float(IN.uv0.xy, _Property_5cb56026df89482f9dd50d0f86f9a4b0_Out_0, _Multiply_7becf085a5d243eca76bdf37a02e12e6_Out_2, _TilingAndOffset_ac94c280ba98466c9da94c75df3c1cf1_Out_3);
                    float4 _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0.tex, _Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0.samplerstate, _Property_c29daaacccf64cc1a29e8bfba8df1cf6_Out_0.GetTransformedUV(_TilingAndOffset_ac94c280ba98466c9da94c75df3c1cf1_Out_3));
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_R_4 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.r;
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_G_5 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.g;
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_B_6 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.b;
                    float _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_A_7 = _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_RGBA_0.a;
                    float2 _Vector2_44cfddc4d47949c2b7c3fb5a266b368e_Out_0 = float2(_SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_R_4, _SampleTexture2D_33a01a47dcd14af4b918e0b433c122e9_G_5);
                    float _Property_b2652f4c92094484963087b5944a70ca_Out_0 = _Distort;
                    float2 _Multiply_65aaa81e0e92400182c41c2fa357113c_Out_2;
                    Unity_Multiply_float2_float2(_Vector2_44cfddc4d47949c2b7c3fb5a266b368e_Out_0, (_Property_b2652f4c92094484963087b5944a70ca_Out_0.xx), _Multiply_65aaa81e0e92400182c41c2fa357113c_Out_2);
                    float2 _Add_048f26e03f754344805d932eff6f795f_Out_2;
                    Unity_Add_float2(_TilingAndOffset_cb43b0e21cbd4c0cb62d5d407d232ee9_Out_3, _Multiply_65aaa81e0e92400182c41c2fa357113c_Out_2, _Add_048f26e03f754344805d932eff6f795f_Out_2);
                    float4 _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e477ef47724940d883bf7c40fee8f91b_Out_0.tex, _Property_e477ef47724940d883bf7c40fee8f91b_Out_0.samplerstate, _Property_e477ef47724940d883bf7c40fee8f91b_Out_0.GetTransformedUV(_Add_048f26e03f754344805d932eff6f795f_Out_2));
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_R_4 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.r;
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_G_5 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.g;
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_B_6 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.b;
                    float _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_A_7 = _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_RGBA_0.a;
                    float _Smoothstep_b424630841a54791b2a554abf052015e_Out_3;
                    Unity_Smoothstep_float(_Split_416551e7c42b47d7bd4bc436e535c963_R_1, _Split_416551e7c42b47d7bd4bc436e535c963_G_2, _SampleTexture2D_1b9bfb74c5ab45999a868d14b8729072_R_4, _Smoothstep_b424630841a54791b2a554abf052015e_Out_3);
                    float _Property_2ece1a2fcaa347ecba9d2346a5d4655b_Out_0 = _Power;
                    float _Power_e166e5c0e67d480bbb8903b8fb04ac08_Out_2;
                    Unity_Power_float(_Smoothstep_b424630841a54791b2a554abf052015e_Out_3, _Property_2ece1a2fcaa347ecba9d2346a5d4655b_Out_0, _Power_e166e5c0e67d480bbb8903b8fb04ac08_Out_2);
                    float4 _Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2;
                    Unity_Multiply_float4_float4(_Property_2a7bc19c81b347b889230d21744aec4e_Out_0, (_Power_e166e5c0e67d480bbb8903b8fb04ac08_Out_2.xxxx), _Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2);
                    UnityTexture2D _Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
                    float4 _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0.tex, _Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0.samplerstate, _Property_2eb1ba091ca6488a99f4676f25b20ef4_Out_0.GetTransformedUV(IN.uv0.xy));
                    float _SampleTexture2D_873db9df805946b884390669ba634753_R_4 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.r;
                    float _SampleTexture2D_873db9df805946b884390669ba634753_G_5 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.g;
                    float _SampleTexture2D_873db9df805946b884390669ba634753_B_6 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.b;
                    float _SampleTexture2D_873db9df805946b884390669ba634753_A_7 = _SampleTexture2D_873db9df805946b884390669ba634753_RGBA_0.a;
                    float4 _Multiply_f25fd46e138f4dfb8f796c1b5a44ec70_Out_2;
                    Unity_Multiply_float4_float4(_Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2, (_SampleTexture2D_873db9df805946b884390669ba634753_R_4.xxxx), _Multiply_f25fd46e138f4dfb8f796c1b5a44ec70_Out_2);
                    float4 _Saturate_63908d978ae0497ca92a1453a374852d_Out_1;
                    Unity_Saturate_float4(_Multiply_f25fd46e138f4dfb8f796c1b5a44ec70_Out_2, _Saturate_63908d978ae0497ca92a1453a374852d_Out_1);
                    float4 _Branch_ca00cede9c6845f8acaff6ba576c90f8_Out_3;
                    Unity_Branch_float4(_Property_1083bf5988a14e99a816880fe20f59c1_Out_0, _Saturate_63908d978ae0497ca92a1453a374852d_Out_1, _Multiply_e3328a397e8a482fb3923e47b5fde55d_Out_2, _Branch_ca00cede9c6845f8acaff6ba576c90f8_Out_3);
                    surface.BaseColor = (_Branch_ca00cede9c6845f8acaff6ba576c90f8_Out_3.xyz);
                    surface.Alpha = 1;
                    return surface;
                }
            
                // --------------------------------------------------
                // Build Graph Inputs
            
                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                
                    return output;
                }
                
                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                    
                
                
                
                
                
                    output.uv0 =                                        input.texCoord0;
                    output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                    return output;
                }
                
            
                // --------------------------------------------------
                // Main
            
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
            
                ENDHLSL
            }
        }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
        FallBack "Hidden/Shader Graph/FallbackError"
    }