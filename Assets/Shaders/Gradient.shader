Shader "Custom/Gradient"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Tint ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "LightMode" = "Always" "RenderType"="Opaque" }
        LOD 200
        Fog { Mode Off }
        ZWrite On
        ZTest LEqual
        Lighting Off
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        fixed4 _Color;
        fixed4 _Color2;
        fixed4 _Tint;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            fixed4 c = lerp(_Color2, _Color, screenUV.y) * _Tint;
            o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
