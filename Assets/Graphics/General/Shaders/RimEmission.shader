// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Unlit/Color,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.6705883,fgcg:0.7019608,fgcb:0.7490196,fgca:1,fgde:0.02,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7845-OUT,alpha-8470-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32031,y:32598,ptovrint:False,ptlb:Rim Color,ptin:_RimColor,varname:_RimColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Dot,id:4079,x:31614,y:32731,varname:node_4079,prsc:2,dt:0|A-4033-OUT,B-7415-OUT;n:type:ShaderForge.SFN_ViewVector,id:4033,x:31428,y:32661,varname:node_4033,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:7415,x:31428,y:32798,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:1806,x:32294,y:32691,varname:node_1806,prsc:2|A-7241-RGB,B-6050-OUT,C-1696-OUT;n:type:ShaderForge.SFN_OneMinus,id:1696,x:32031,y:32926,varname:node_1696,prsc:2|IN-9789-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6050,x:32031,y:32782,ptovrint:False,ptlb:Rim Emission,ptin:_RimEmission,varname:_RimEmission,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Add,id:7845,x:32482,y:32785,varname:node_7845,prsc:2|A-1806-OUT,B-6546-RGB;n:type:ShaderForge.SFN_Color,id:6546,x:32294,y:32874,ptovrint:False,ptlb:Inner Color,ptin:_InnerColor,varname:_BodyColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:0.5;n:type:ShaderForge.SFN_Clamp01,id:9789,x:31837,y:32926,varname:node_9789,prsc:2|IN-7888-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8736,x:31614,y:32908,ptovrint:False,ptlb:Rim Power,ptin:_RimPower,varname:_RimThickness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:7888,x:31837,y:32782,varname:node_7888,prsc:2|VAL-4079-OUT,EXP-8736-OUT;n:type:ShaderForge.SFN_Add,id:2297,x:32294,y:33054,varname:node_2297,prsc:2|A-6546-A,B-1696-OUT;n:type:ShaderForge.SFN_Clamp01,id:8470,x:32482,y:33054,varname:node_8470,prsc:2|IN-2297-OUT;proporder:6546-7241-6050-8736;pass:END;sub:END;*/

Shader "Shader Forge/Rim Emission" {
    Properties {
        _InnerColor ("Inner Color", Color) = (1,0,0,0.5)
        _RimColor ("Rim Color", Color) = (1,0,0,1)
        _RimEmission ("Rim Emission", Float ) = 10
        _RimPower ("Rim Power", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _RimColor;
            uniform float _RimEmission;
            uniform float4 _InnerColor;
            uniform float _RimPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_1696 = (1.0 - saturate(pow(dot(viewDirection,i.normalDir),_RimPower)));
                float3 emissive = ((_RimColor.rgb*_RimEmission*node_1696)+_InnerColor.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,saturate((_InnerColor.a+node_1696)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
    CustomEditor "ShaderForgeMaterialInspector"
}
