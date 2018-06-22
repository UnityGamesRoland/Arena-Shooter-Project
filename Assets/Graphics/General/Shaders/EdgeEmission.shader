// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32820,y:32667,varname:node_3138,prsc:2|emission-1931-OUT,alpha-6787-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31892,y:32525,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_TexCoord,id:314,x:29976,y:32811,varname:node_314,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ComponentMask,id:9247,x:30181,y:32719,varname:node_9247,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-314-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:9366,x:30181,y:32990,varname:node_9366,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-314-UVOUT;n:type:ShaderForge.SFN_Vector1,id:8250,x:30181,y:32655,varname:node_8250,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:5138,x:30181,y:33139,varname:node_5138,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:6752,x:30380,y:32694,varname:node_6752,prsc:2|A-8250-OUT,B-9247-OUT,C-5100-OUT;n:type:ShaderForge.SFN_Multiply,id:5581,x:30380,y:32950,varname:node_5581,prsc:2|A-5100-OUT,B-9366-OUT,C-5138-OUT;n:type:ShaderForge.SFN_Sin,id:934,x:30567,y:32694,varname:node_934,prsc:2|IN-6752-OUT;n:type:ShaderForge.SFN_Sin,id:3676,x:30567,y:32950,varname:node_3676,prsc:2|IN-5581-OUT;n:type:ShaderForge.SFN_OneMinus,id:4783,x:30776,y:32694,varname:node_4783,prsc:2|IN-934-OUT;n:type:ShaderForge.SFN_OneMinus,id:9325,x:30776,y:32950,varname:node_9325,prsc:2|IN-3676-OUT;n:type:ShaderForge.SFN_Power,id:7090,x:31251,y:32706,varname:node_7090,prsc:2|VAL-6116-OUT,EXP-883-OUT;n:type:ShaderForge.SFN_Power,id:931,x:31251,y:32962,varname:node_931,prsc:2|VAL-5985-OUT,EXP-883-OUT;n:type:ShaderForge.SFN_Subtract,id:2233,x:31475,y:32706,varname:node_2233,prsc:2|A-7090-OUT,B-9256-OUT;n:type:ShaderForge.SFN_Vector1,id:9256,x:31251,y:32860,varname:node_9256,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Subtract,id:4180,x:31475,y:32962,varname:node_4180,prsc:2|A-931-OUT,B-9256-OUT;n:type:ShaderForge.SFN_Lerp,id:9424,x:31712,y:32826,varname:node_9424,prsc:2|A-2233-OUT,B-4180-OUT,T-6770-OUT;n:type:ShaderForge.SFN_Tau,id:5100,x:30198,y:32871,varname:node_5100,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:5985,x:30987,y:32950,varname:node_5985,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9325-OUT;n:type:ShaderForge.SFN_RemapRange,id:6116,x:30987,y:32694,varname:node_6116,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-4783-OUT;n:type:ShaderForge.SFN_Multiply,id:6335,x:32320,y:32649,varname:node_6335,prsc:2|A-7241-RGB,B-9855-OUT,C-3390-OUT;n:type:ShaderForge.SFN_Clamp01,id:7579,x:31892,y:32826,varname:node_7579,prsc:2|IN-9424-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6770,x:31475,y:32880,ptovrint:False,ptlb:Edge Direction,ptin:_EdgeDirection,varname:node_6770,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Step,id:2573,x:32115,y:32854,varname:node_2573,prsc:2|A-7579-OUT,B-8429-OUT;n:type:ShaderForge.SFN_Vector1,id:8429,x:31892,y:32958,varname:node_8429,prsc:2,v1:0.3;n:type:ShaderForge.SFN_OneMinus,id:3390,x:32320,y:32854,varname:node_3390,prsc:2|IN-2573-OUT;n:type:ShaderForge.SFN_ValueProperty,id:883,x:30987,y:32883,ptovrint:False,ptlb:Edge Width,ptin:_EdgeWidth,varname:node_883,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:15;n:type:ShaderForge.SFN_Add,id:6787,x:32556,y:32920,varname:node_6787,prsc:2|A-3390-OUT,B-470-OUT;n:type:ShaderForge.SFN_Add,id:1931,x:32556,y:32590,varname:node_1931,prsc:2|A-4195-OUT,B-6335-OUT;n:type:ShaderForge.SFN_Slider,id:470,x:32163,y:33026,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:1;n:type:ShaderForge.SFN_Multiply,id:4195,x:32320,y:32417,varname:node_4195,prsc:2|A-8542-OUT,B-9522-RGB;n:type:ShaderForge.SFN_Fresnel,id:8542,x:32122,y:32261,varname:node_8542,prsc:2|NRM-5282-OUT,EXP-4957-OUT;n:type:ShaderForge.SFN_NormalVector,id:5282,x:31889,y:32191,prsc:2,pt:False;n:type:ShaderForge.SFN_Color,id:9522,x:32122,y:32417,ptovrint:False,ptlb:Fresnel Color,ptin:_FresnelColor,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:4957,x:31889,y:32364,ptovrint:False,ptlb:Fresnel Power,ptin:_FresnelPower,varname:node_4957,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.6;n:type:ShaderForge.SFN_Slider,id:9855,x:31735,y:32710,ptovrint:False,ptlb:Emission Intensity,ptin:_EmissionIntensity,varname:_Opacity_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;proporder:7241-9522-470-9855-883-6770-4957;pass:END;sub:END;*/

Shader "Shader Forge/EdgeEmission" {
    Properties {
        _Color ("Color", Color) = (1,0,0,1)
        _FresnelColor ("Fresnel Color", Color) = (1,0,0,1)
        _Opacity ("Opacity", Range(0, 1)) = 0.25
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1
        _EdgeWidth ("Edge Width", Float ) = 15
        _EdgeDirection ("Edge Direction", Float ) = 0.5
        _FresnelPower ("Fresnel Power", Float ) = 0.6
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _EdgeDirection;
            uniform float _EdgeWidth;
            uniform float _Opacity;
            uniform float4 _FresnelColor;
            uniform float _FresnelPower;
            uniform float _EmissionIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_5100 = 6.28318530718;
                float node_9256 = 0.1;
                float node_3390 = (1.0 - step(saturate(lerp((pow(((1.0 - sin((0.5*i.uv0.r*node_5100)))*0.5+0.5),_EdgeWidth)-node_9256),(pow(((1.0 - sin((node_5100*i.uv0.g*0.5)))*0.5+0.5),_EdgeWidth)-node_9256),_EdgeDirection)),0.3));
                float3 emissive = ((pow(1.0-max(0,dot(i.normalDir, viewDirection)),_FresnelPower)*_FresnelColor.rgb)+(_Color.rgb*_EmissionIntensity*node_3390));
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_3390+_Opacity));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
