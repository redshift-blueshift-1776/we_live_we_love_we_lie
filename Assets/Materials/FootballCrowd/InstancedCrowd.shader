Shader "Custom/ProceduralCrowd"
{
    Properties
    {
        _ShirtColor ("Shirt Color", Color) = (0.2,0.5,1,1)
        _PantsColor ("Pants Color", Color) = (0.1,0.1,0.1,1)
        _SkinColor ("Skin Color", Color) = (1,0.8,0.6,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)

            UNITY_DEFINE_INSTANCED_PROP(float4, _ShirtColor)
            UNITY_DEFINE_INSTANCED_PROP(float4, _PantsColor)
            UNITY_DEFINE_INSTANCED_PROP(float4, _SkinColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _HeightOffset)

            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float heightOffset =
                    UNITY_ACCESS_INSTANCED_PROP(Props, _HeightOffset);

                float wave =
                    sin(_Time.y * 3 + heightOffset * 10) * 0.03;

                float4 pos = v.vertex;

                pos.y += wave;

                o.pos = UnityObjectToClipPos(pos);
                o.uv = v.uv;

                return o;
            }

            float rect(float2 uv, float2 pos, float2 size)
            {
                float2 d = abs(uv - pos) - size;
                return step(max(d.x, d.y), 0);
            }

            float circle(float2 uv, float2 pos, float r)
            {
                return step(distance(uv, pos), r);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float2 uv = i.uv;

                fixed4 shirt =
                    UNITY_ACCESS_INSTANCED_PROP(Props, _ShirtColor);

                fixed4 pants =
                    UNITY_ACCESS_INSTANCED_PROP(Props, _PantsColor);

                fixed4 skin =
                    UNITY_ACCESS_INSTANCED_PROP(Props, _SkinColor);

                float head =
                    circle(uv, float2(0.5, 0.78), 0.12);

                float torso =
                    rect(uv, float2(0.5, 0.52), float2(0.13, 0.18));

                float leg1 =
                    rect(uv, float2(0.44, 0.22), float2(0.05, 0.16));

                float leg2 =
                    rect(uv, float2(0.56, 0.22), float2(0.05, 0.16));

                float arm1 =
                    rect(uv, float2(0.30, 0.50), float2(0.10, 0.04));

                float arm2 =
                    rect(uv, float2(0.70, 0.50), float2(0.10, 0.04));

                fixed4 col = fixed4(0,0,0,0);

                if (head > 0)
                    col = skin;

                if (torso > 0 || arm1 > 0 || arm2 > 0)
                    col = shirt;

                if (leg1 > 0 || leg2 > 0)
                    col = pants;

                clip(col.a - 0.01);

                return col;
            }

            ENDHLSL
        }
    }
}