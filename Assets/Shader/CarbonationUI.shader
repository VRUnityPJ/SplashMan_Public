Shader "UI/CarbonationOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _BubbleColor ("Bubble Color", Color) = (0.85, 0.97, 1, 1)
        _BubbleDensity ("Bubble Density", Range(4, 40)) = 18
        _BubbleAspect ("Rect Width / Height", Float) = 1
        _BubbleSize ("Bubble Size", Range(0.1, 0.45)) = 0.25
        _BubbleSpeed ("Rise Speed", Range(0, 3)) = 0.6
        _BubbleStrength ("Bubble Strength", Range(0, 1)) = 0.8

        // Unity UI stencil / Mask support
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TextureSampleAdd;
            float4 _Color;
            float4 _BubbleColor;
            float4 _ClipRect;

            float _BubbleDensity;
            float _BubbleAspect;
            float _BubbleSize;
            float _BubbleSpeed;
            float _BubbleStrength;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            v2f vert(appdata_t v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Original UI texture and tint
                fixed4 baseColor =
                    (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd)
                    * i.color;

                // Animate a procedural bubble grid upward.
                float2 p = i.texcoord
                    * float2(
                        _BubbleDensity * max(_BubbleAspect, 0.01),
                        _BubbleDensity
                    );

                p.y -= _Time.y * _BubbleSpeed;

                float2 cell = floor(p);
                float2 local = frac(p) - 0.5;

                // Random center and radius for each cell
                float2 rnd = float2(
                    hash21(cell + 1.7),
                    hash21(cell + 8.3)
                );

                float2 center = (rnd - 0.5) * 0.45;
                float radius = _BubbleSize
                    * lerp(0.55, 1.0, hash21(cell + 4.1))
                    * 0.5;

                float2 q = local - center;
                float d = length(q);

                // Thin, soft glass-like rim
                float rimWidth = 0.035;
                float rim = 1.0 - smoothstep(
                    rimWidth, rimWidth + 0.025,
                    abs(d - radius)
                );

                // Small highlight toward the upper-left
                float2 highlightOffset =
                    float2(-radius * 0.32, radius * 0.35);

                float highlight = 1.0 - smoothstep(
                    radius * 0.12,
                    radius * 0.48,
                    length(q - highlightOffset)
                );

                // Very faint inner glow
                float inside = 1.0 - smoothstep(
                    radius * 0.45, radius, d
                );

                float bubble = saturate(
                    rim * 0.75 +
                    highlight * 0.9 +
                    inside * 0.08
                );

                // Fade near the edge of each cell to reduce hard pops.
                // The main rim remains crisp enough to read as a bubble.
                float edgeFade = smoothstep(
                    0.0, 0.12,
                    min(
                        min(frac(p).x, 1.0 - frac(p).x),
                        min(frac(p).y, 1.0 - frac(p).y)
                    )
                );

                bubble *= edgeFade;

                // Only tint visible pixels of the original UI image.
                float bubbleAlpha = bubble
                    * _BubbleStrength
                    * _BubbleColor.a
                    * baseColor.a;

                baseColor.rgb = lerp(
                    baseColor.rgb,
                    max(baseColor.rgb, _BubbleColor.rgb),
                    saturate(bubbleAlpha)
                );

                #ifdef UNITY_UI_CLIP_RECT
                baseColor.a *= UnityGet2DClipping(
                    i.worldPosition.xy, _ClipRect
                );
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(baseColor.a - 0.001);
                #endif

                return baseColor;
            }
            ENDCG
        }
    }
}