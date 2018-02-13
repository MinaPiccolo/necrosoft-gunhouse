Shader "Gunhouse/Sprite" {
    Properties {
        [PerRendererData] _MainTex("RGBA Texture", 2D) = "white" { }
        [PerRendererData] _UniBW("Effect amount", Float) = 0.0
        [PerRendererData] _UniPulseAmtX("Pulse amount", Float) = 0.0
        [PerRendererData] _UniPulseAmtY("Pulse amount", Float) = 0.0
        [PerRendererData] _UniAmp("Color amplify", Float) = 0.0
        [PerRendererData] _OutlineSize("Outline Size", Float) = 0
        [PerRendererData] _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
    }

    Category {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }

        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
            Bind "TexCoord", texcoord
            Bind "TexCoord1", texcoord1
            Bind "TexCoord2", texcoord2
        }

        SubShader {
            Pass {
                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                uniform sampler2D_half _MainTex;
                uniform fixed _UniBW;
                uniform fixed _UniPulseAmtX;
                uniform fixed _UniPulseAmtY;
                uniform fixed _UniAmp;

                uniform fixed _OutlineSize;
                uniform fixed4 _OutlineColor;
                uniform fixed4 _MainTex_TexelSize;

                struct v2f {
                    fixed4 pos : SV_POSITION;
                    fixed4 diff : COLOR0;
                    fixed2 uv : TEXCOORD0;
                    fixed4 square : TEXCOORD1;
                };

                v2f vert(appdata_full v) {
                    v2f o;

                    fixed4 drift = fixed4(v.vertex.x + sin(v.vertex.x / 10.0f) * _UniPulseAmtX,
                                          v.vertex.y + cos(v.vertex.y / 10.0f) * _UniPulseAmtY,
                                          0.0f, 1.0f);
                    o.pos = UnityObjectToClipPos(drift);
                    o.diff = v.color;
                    o.uv = v.texcoord;
                    o.square.xy = (v.vertex.xy - v.texcoord1) / _OutlineSize;
                    o.square.zw = (v.texcoord2 - v.vertex.xy + v.texcoord1) / _OutlineSize;
                    return o;
                }

                fixed4 frag(v2f pixel) : COLOR
                {
                    fixed4 result = tex2D(_MainTex, pixel.uv) * (pixel.diff * (1 + _UniAmp));
                    if (_UniBW > 0.1) { result.rgb = dot(result.rgb, fixed3(0.34f, 0.34f, 0.34f)); }

                    if (_OutlineColor.a < 0.0001) return result;

                    fixed edge0 = step(1, pixel.square.x);
                    fixed edge1 = step(1, pixel.square.y);
                    fixed edge2 = step(1, pixel.square.z);
                    fixed edge3 = step(1, pixel.square.w);
                    result.rgb = lerp(result.rgb, _OutlineColor.rgb, _OutlineColor.a * (1.0 - edge0 * edge1 * edge2 * edge3));

                    return result;
                }

                ENDCG
            }
        }
    }
}