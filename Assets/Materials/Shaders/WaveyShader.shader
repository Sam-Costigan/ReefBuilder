﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Wavey Shader"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		_CausticsStartLevel("Caustics Start Level", Float) = 0.0
		_CausticsShallowFadeDistance("Caustics Shallow Distance", Float) = 1.0
		_CausticsScale("Caustics Scale", Float) = 1.0
		_CausticsDrift("Caustics Drift", Vector) = (0.1, 0.0, -0.4)
		//_Health("Health", Float) = 1.0 

    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode"="ForwardBase"}

        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
			#include "UnityStandardCausticsCore.cginc"

            //uniform fixed3 unity_FogColor;
        	uniform half unity_FogDensity;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1)
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = v.vertex;

                if(v.vertex.y > 0){
                	float y = v.vertex.y;

                	o.vertex.x += 0.1f * sin(_Time.y*0.5f + (y*2.0f)) * (y/2.0f);
                	o.vertex.z += 0.1f * sin(_Time.y*0.5f + (y*2.0f)) * (y/2.0f);
                	//v.vertex.z -= _SinTime.w * (v.vertex.z/10.0f) * 20.0f;
                }
                o.vertex = UnityObjectToClipPos(o.vertex);


                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;

                o.diff.rgb += ShadeSH9(half4(worldNormal,1));
                o.diff.rgb *= 3.0f;

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }
            
            //sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.diff;

                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);

				col += _EmissionColor;

                return col;
            }
            ENDCG
        }
    }
}