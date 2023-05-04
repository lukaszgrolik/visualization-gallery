Shader "Lukasz/GridShader"
{
    // @todo transparent bg color // or semi-transparent - user can define bg color
    // @todo semi-transparent line color
    // @todo rows and columns - don't blend colors at intersections

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CellsPerRow ("Cells Per Row", Float) = 10
        _LineSize ("Line Size", Float) = 0.01
        _BackgroundColor ("Background Color", Color) = (0,0,0,0)
        _LineColor ("Line Color", Color) = (0,0,0,0.2)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            // "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            // "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Cull Back // Back, Front, Off
            ZWrite Off
            ZTest LEqual // LEqual (default), Always, GEqual // GEqual - for a character beyond and obstacle
            // Blend One One // additive
            // Blend DstColor Zero // multiplicative
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./ShaderExtensions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                // OUT.normal = v.normal;
                OUT.normal = UnityObjectToWorldNormal(v.normal);
                OUT.uv = v.uv;

                return OUT;
            }

            sampler2D _MainTex;
            float _CellsPerRow;
            float _LineSize;
            float4 _BackgroundColor;
            float4 _LineColor;

            float GridLine(float pos, float n, float lineSize)
            {
                float x = roundPrec(pos, 1 / n);

                return abs(x - pos) <= lineSize / 2;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float pattern_y = GridLine(IN.uv.x, _CellsPerRow, _LineSize);
                float pattern_x = GridLine(IN.uv.y, _CellsPerRow, _LineSize);
                float pattern = pattern_y == 1 || pattern_x == 1;

                return pattern == 0 ? _BackgroundColor : _LineColor;
            }
            ENDCG
        }
    }
}
