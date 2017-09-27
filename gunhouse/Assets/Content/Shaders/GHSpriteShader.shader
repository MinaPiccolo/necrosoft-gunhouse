Shader "Gunhouse/Sprite" {
    Properties {
        [PerRendererData] _MainTex("RGBA Texture", 2D) = "white" { }
        [PerRendererData] _UniBW("Effect amount", Float) = 0.0
        [PerRendererData] _UniPulseAmtX("Pulse amount", Float) = 0.0
        [PerRendererData] _UniPulseAmtY("Pulse amount", Float) = 0.0
        [PerRendererData] _UniAmp("Color amplify", Float) = 0.0
        [PerRendererData] _OutlineSize("Outline Size", int) = 0
        [PerRendererData] _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
    }

    Category {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off Fog { Color(0, 0, 0, 0) }

        BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
        Bind "TexCoord1", texcoord1
        Bind "TexCoord2", texcoord2 }
        SubShader {
            Pass {
                CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

                uniform sampler2D _MainTex;
                uniform float _UniBW;
                uniform float _UniPulseAmtX;
                uniform float _UniPulseAmtY;
                uniform float _UniAmp;

                uniform int _OutlineSize;
                uniform fixed4 _OutlineColor;
                uniform float4 _MainTex_TexelSize;

                struct v2f {
                    float4 pos : SV_POSITION;
                    fixed4 diff : COLOR0;
                    float2 uv : TEXCOORD0;
                    float4 square : TEXCOORD1;
                };

                v2f vert(appdata_full v) {
                    v2f o;

                    float4 drift = float4(v.vertex.x + sin(v.vertex.x / 10.0f) * _UniPulseAmtX,
                                          v.vertex.y + cos(v.vertex.y / 10.0f) * _UniPulseAmtY,
                                          0.0f, 1.0f);
                    o.pos = UnityObjectToClipPos(drift);
                    o.diff = v.color;
                    o.uv = v.texcoord;
                    o.square.xy = (v.vertex.xy - v.texcoord1) / float(_OutlineSize);
                    o.square.zw = (v.texcoord2 - v.vertex.xy + v.texcoord1) / float(_OutlineSize);
                    return o;
                }

                float4 frag(v2f pixel) : COLOR
                {
                    float4 tinted = tex2D(_MainTex, float2(pixel.uv.x, pixel.uv.y)) * (pixel.diff * (1 + _UniAmp));
                    float value = (tinted.r + tinted.g + tinted.b) / 3.0f;
                    float4 result = float4(value, value, value, tinted.a);
                    result = (result * _UniBW) + tinted * (1.0f - _UniBW);

                    if (_OutlineColor.a < 0.0001) return result;

                    float edge0 = step(1, pixel.square.x);
                    float edge1 = step(1, pixel.square.y);
                    float edge2 = step(1, pixel.square.z);
                    float edge3 = step(1, pixel.square.w);
                    result.rgb = lerp(result.rgb, _OutlineColor.rgb, _OutlineColor.a*(1.0 - edge0 * edge1 * edge2 * edge3));

                    return result;
                }

                ENDCG
            }
        }
    }
}
