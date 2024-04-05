Shader "Custom/TestBlinn-PhongLight" {
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _AlbedoColor ("AlbedoColor", Color) = (1, 1, 1, 1)
        _AmbientColor("AmbientColor", Color) = (1, 1, 1, 1)
        _AmbientStrength("AmbientStrength",Range(0,1)) = 0.2
        _SpecularColor("SpecularColor", Color) = (1, 1, 1, 1)
        _SpecularStrength("SpecularStrength",Range(0,1)) = 1
        _Shininess ("Shininess", Range(1, 128)) = 32
    }
 
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 pos : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AlbedoColor;
            float4 _AmbientColor;
            float _AmbientStrength;
            float4 _SpecularColor;
            float _SpecularStrength;
            float _Shininess;
 
            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                return o;
            }
 
            half4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.pos);
                //float3 reflectDir = reflect(-lightDir, i.normal);
                half3 halfDir = normalize(viewDir + lightDir);
                
                half4 diffuse = tex * _AlbedoColor * max(0, dot(i.normal, lightDir));
                float ambient = _AmbientColor.rgb * _AmbientStrength;
                //float specular = _SpecularColor.rgb * pow(max(0, dot(reflectDir, viewDir)), _Shininess) * _SpecularStrength;
                float specular = _SpecularColor.rgb * pow(max(0, dot(i.normal, halfDir)), _Shininess) * _SpecularStrength;
                half4 col = diffuse + ambient + specular;
                return col;
            }
            ENDCG
        }
    }
}
