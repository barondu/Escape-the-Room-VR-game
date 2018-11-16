// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//GaussianBlur_LiveBlur.shader
//This is the first version of the shader for people who still want to use it.

Shader "Hidden/GaussianBlur_RenderBlur" 
{
    Properties 
    {

		_MainTex ("MainTex", 2D) = "white" {}
        _BlurSize ("BlurSize", Range(0, 100)) = 25
        _Quality ("Quality", Range(0,4)) = 2
		// _Lightness ("_Lightness", Range(-1,1)) = 0
    }
 
    Category 
    {

        Tags
         { 
             "Queue"="Transparent"  
             "RenderType"="Transparent" 
             "PreviewType"="Plane"
             "CanUseSpriteAtlas"="True"
         }

        SubShader 
        {
     
            // Vertical blur
            Pass 
            {
                Tags { "LightMode" = "Always" }
				ZTest Always Cull Off ZWrite Off
				Fog { Mode off }
             
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t 
                {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f 
                {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 mask : TEXCOORD1; 
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v) 
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.mask = TRANSFORM_TEX( v.texcoord, _MainTex );
                    return o;
                }
             
                float _BlurSize;
                int _Quality;
                sampler2D _MainTex;
				float4 _MainTex_TexelSize;

                half4 frag( v2f i ) : COLOR 
                {

                    half4 sum = half4(0,0,0,0);
                    #define GRABPIXEL(weight,kernely) tex2Dproj( _MainTex, UNITY_PROJ_COORD(float4(i.uvgrab.x , i.uvgrab.y + _MainTex_TexelSize.x * kernely * _BlurSize, i.uvgrab.z, i.uvgrab.w))) * weight

                    if (_Quality == 0)
                    {
	                    sum += GRABPIXEL(0.05000,-4.0000);
						sum += GRABPIXEL(0.09000,-3.0000);
						sum += GRABPIXEL(0.12000,-2.0000);
						sum += GRABPIXEL(0.15000,-1.0000);
						sum += GRABPIXEL(0.18000,0.0000);
						sum += GRABPIXEL(0.15000,1.0000);
						sum += GRABPIXEL(0.12000,2.0000);
						sum += GRABPIXEL(0.09000,3.0000);
						sum += GRABPIXEL(0.05000,4.0000);

						sum /= (  1 );
                    }
                    else if (_Quality == 1)
                    {
						sum += GRABPIXEL(0.0500,-4.000);
						sum += GRABPIXEL(0.0700,-3.500);
						sum += GRABPIXEL(0.0900,-3.000);
						sum += GRABPIXEL(0.1050,-2.500);
						sum += GRABPIXEL(0.1200,-2.000);
						sum += GRABPIXEL(0.1350,-1.500);
						sum += GRABPIXEL(0.1500,-1.000);
						sum += GRABPIXEL(0.1650,-0.500);
						sum += GRABPIXEL(0.1800,0.000);
						sum += GRABPIXEL(0.1650,0.500);
						sum += GRABPIXEL(0.1500,1.000);
						sum += GRABPIXEL(0.1350,1.500);
						sum += GRABPIXEL(0.1200,2.000);
						sum += GRABPIXEL(0.1050,2.500);
						sum += GRABPIXEL(0.0900,3.000);
						sum += GRABPIXEL(0.0700,3.500);
						sum += GRABPIXEL(0.0500,4.000);

						sum /= (  1.95);
                    }
                    else if (_Quality == 2)
                    {
						sum += GRABPIXEL(0.0500,-4.000);
						sum += GRABPIXEL(0.0600,-3.750);
						sum += GRABPIXEL(0.0700,-3.500);
						sum += GRABPIXEL(0.0800,-3.250);
						sum += GRABPIXEL(0.0900,-3.000);
						sum += GRABPIXEL(0.0975,-2.750);
						sum += GRABPIXEL(0.1050,-2.500);
						sum += GRABPIXEL(0.1125,-2.250);
						sum += GRABPIXEL(0.1200,-2.000);
						sum += GRABPIXEL(0.1275,-1.750);
						sum += GRABPIXEL(0.1350,-1.500);
						sum += GRABPIXEL(0.1425,-1.250);
						sum += GRABPIXEL(0.1500,-1.000);
						sum += GRABPIXEL(0.1575,-0.750);
						sum += GRABPIXEL(0.1650,-0.500);
						sum += GRABPIXEL(0.1725,-0.250);
						sum += GRABPIXEL(0.1800,0.000);
						sum += GRABPIXEL(0.1725,0.250);
						sum += GRABPIXEL(0.1650,0.500);
						sum += GRABPIXEL(0.1575,0.750);
						sum += GRABPIXEL(0.1500,1.000);
						sum += GRABPIXEL(0.1425,1.250);
						sum += GRABPIXEL(0.1350,1.500);
						sum += GRABPIXEL(0.1275,1.750);
						sum += GRABPIXEL(0.1200,2.000);
						sum += GRABPIXEL(0.1125,2.250);
						sum += GRABPIXEL(0.1050,2.500);
						sum += GRABPIXEL(0.0975,2.750);
						sum += GRABPIXEL(0.0900,3.000);
						sum += GRABPIXEL(0.0800,3.250);
						sum += GRABPIXEL(0.0700,3.500);
						sum += GRABPIXEL(0.0600,3.750);
						sum += GRABPIXEL(0.0500,4.000);

						sum /= (  3.85);
                    }
                    else if (_Quality == 3)
                    {
						sum += GRABPIXEL(0.0500,-4.000);
						sum += GRABPIXEL(0.0550,-3.875);
						sum += GRABPIXEL(0.0600,-3.750);
						sum += GRABPIXEL(0.0650,-3.625);
						sum += GRABPIXEL(0.0700,-3.500);
						sum += GRABPIXEL(0.0750,-3.375);
						sum += GRABPIXEL(0.0800,-3.250);
						sum += GRABPIXEL(0.0850,-3.125);
						sum += GRABPIXEL(0.0900,-3.000);
						sum += GRABPIXEL(0.0938,-2.875);
						sum += GRABPIXEL(0.0975,-2.750);
						sum += GRABPIXEL(0.1013,-2.625);
						sum += GRABPIXEL(0.1050,-2.500);
						sum += GRABPIXEL(0.1088,-2.375);
						sum += GRABPIXEL(0.1125,-2.250);
						sum += GRABPIXEL(0.1163,-2.125);
						sum += GRABPIXEL(0.1200,-2.000);
						sum += GRABPIXEL(0.1238,-1.875);
						sum += GRABPIXEL(0.1275,-1.750);
						sum += GRABPIXEL(0.1313,-1.625);
						sum += GRABPIXEL(0.1350,-1.500);
						sum += GRABPIXEL(0.1388,-1.375);
						sum += GRABPIXEL(0.1425,-1.250);
						sum += GRABPIXEL(0.1463,-1.125);
						sum += GRABPIXEL(0.1500,-1.000);
						sum += GRABPIXEL(0.1538,-0.875);
						sum += GRABPIXEL(0.1575,-0.750);
						sum += GRABPIXEL(0.1613,-0.625);
						sum += GRABPIXEL(0.1650,-0.500);
						sum += GRABPIXEL(0.1688,-0.375);
						sum += GRABPIXEL(0.1725,-0.250);
						sum += GRABPIXEL(0.1763,-0.125);
						sum += GRABPIXEL(0.1800,0.000);
						sum += GRABPIXEL(0.1763,0.125);
						sum += GRABPIXEL(0.1725,0.250);
						sum += GRABPIXEL(0.1688,0.375);
						sum += GRABPIXEL(0.1650,0.500);
						sum += GRABPIXEL(0.1613,0.625);
						sum += GRABPIXEL(0.1575,0.750);
						sum += GRABPIXEL(0.1538,0.875);
						sum += GRABPIXEL(0.1500,1.000);
						sum += GRABPIXEL(0.1463,1.125);
						sum += GRABPIXEL(0.1425,1.250);
						sum += GRABPIXEL(0.1388,1.375);
						sum += GRABPIXEL(0.1350,1.500);
						sum += GRABPIXEL(0.1313,1.625);
						sum += GRABPIXEL(0.1275,1.750);
						sum += GRABPIXEL(0.1238,1.875);
						sum += GRABPIXEL(0.1200,2.000);
						sum += GRABPIXEL(0.1163,2.125);
						sum += GRABPIXEL(0.1125,2.250);
						sum += GRABPIXEL(0.1088,2.375);
						sum += GRABPIXEL(0.1050,2.500);
						sum += GRABPIXEL(0.1013,2.625);
						sum += GRABPIXEL(0.0975,2.750);
						sum += GRABPIXEL(0.0938,2.875);
						sum += GRABPIXEL(0.0900,3.000);
						sum += GRABPIXEL(0.0850,3.125);
						sum += GRABPIXEL(0.0800,3.250);
						sum += GRABPIXEL(0.0750,3.375);
						sum += GRABPIXEL(0.0700,3.500);
						sum += GRABPIXEL(0.0650,3.625);
						sum += GRABPIXEL(0.0600,3.750);
						sum += GRABPIXEL(0.0550,3.875);
						sum += GRABPIXEL(0.0500,4.000);

						sum /= (  7.65);
                    }
                    else if (_Quality >= 4)
                    {

                        sum += GRABPIXEL(0.0500,-4.000);
                        sum += GRABPIXEL(0.0525,-3.938);
                        sum += GRABPIXEL(0.0550,-3.875);
                        sum += GRABPIXEL(0.0575,-3.813);
                        sum += GRABPIXEL(0.0600,-3.750);
                        sum += GRABPIXEL(0.0625,-3.688);
                        sum += GRABPIXEL(0.0650,-3.625);
                        sum += GRABPIXEL(0.0675,-3.563);
                        sum += GRABPIXEL(0.0700,-3.500);
                        sum += GRABPIXEL(0.0725,-3.438);
                        sum += GRABPIXEL(0.0750,-3.375);
                        sum += GRABPIXEL(0.0775,-3.313);
                        sum += GRABPIXEL(0.0800,-3.250);
                        sum += GRABPIXEL(0.0825,-3.188);
                        sum += GRABPIXEL(0.0850,-3.125);
                        sum += GRABPIXEL(0.0875,-3.063);
                        sum += GRABPIXEL(0.0900,-3.000);
                        sum += GRABPIXEL(0.0919,-2.938);
                        sum += GRABPIXEL(0.0938,-2.875);
                        sum += GRABPIXEL(0.0956,-2.813);
                        sum += GRABPIXEL(0.0975,-2.750);
                        sum += GRABPIXEL(0.0994,-2.688);
                        sum += GRABPIXEL(0.1013,-2.625);
                        sum += GRABPIXEL(0.1031,-2.563);
                        sum += GRABPIXEL(0.1050,-2.500);
                        sum += GRABPIXEL(0.1069,-2.438);
                        sum += GRABPIXEL(0.1088,-2.375);
                        sum += GRABPIXEL(0.1106,-2.313);
                        sum += GRABPIXEL(0.1125,-2.250);
                        sum += GRABPIXEL(0.1144,-2.188);
                        sum += GRABPIXEL(0.1163,-2.125);
                        sum += GRABPIXEL(0.1181,-2.063);
                        sum += GRABPIXEL(0.1200,-2.000);
                        sum += GRABPIXEL(0.1219,-1.938);
                        sum += GRABPIXEL(0.1238,-1.875);
                        sum += GRABPIXEL(0.1256,-1.813);
                        sum += GRABPIXEL(0.1275,-1.750);
                        sum += GRABPIXEL(0.1294,-1.688);
                        sum += GRABPIXEL(0.1313,-1.625);
                        sum += GRABPIXEL(0.1331,-1.563);
                        sum += GRABPIXEL(0.1350,-1.500);
                        sum += GRABPIXEL(0.1369,-1.438);
                        sum += GRABPIXEL(0.1388,-1.375);
                        sum += GRABPIXEL(0.1406,-1.313);
                        sum += GRABPIXEL(0.1425,-1.250);
                        sum += GRABPIXEL(0.1444,-1.188);
                        sum += GRABPIXEL(0.1463,-1.125);
                        sum += GRABPIXEL(0.1481,-1.063);
                        sum += GRABPIXEL(0.1500,-1.000);
                        sum += GRABPIXEL(0.1519,-0.938);
                        sum += GRABPIXEL(0.1538,-0.875);
                        sum += GRABPIXEL(0.1556,-0.813);
                        sum += GRABPIXEL(0.1575,-0.750);
                        sum += GRABPIXEL(0.1594,-0.688);
                        sum += GRABPIXEL(0.1613,-0.625);
                        sum += GRABPIXEL(0.1631,-0.563);
                        sum += GRABPIXEL(0.1650,-0.500);
                        sum += GRABPIXEL(0.1669,-0.438);
                        sum += GRABPIXEL(0.1688,-0.375);
                        sum += GRABPIXEL(0.1706,-0.313);
                        sum += GRABPIXEL(0.1725,-0.250);
                        sum += GRABPIXEL(0.1744,-0.188);
                        sum += GRABPIXEL(0.1763,-0.125);
                        sum += GRABPIXEL(0.1781,-0.063);
                        sum += GRABPIXEL(0.1800,0.000);
                        sum += GRABPIXEL(0.1781,0.063);
                        sum += GRABPIXEL(0.1763,0.125);
                        sum += GRABPIXEL(0.1744,0.188);
                        sum += GRABPIXEL(0.1725,0.250);
                        sum += GRABPIXEL(0.1706,0.313);
                        sum += GRABPIXEL(0.1688,0.375);
                        sum += GRABPIXEL(0.1669,0.438);
                        sum += GRABPIXEL(0.1650,0.500);
                        sum += GRABPIXEL(0.1631,0.563);
                        sum += GRABPIXEL(0.1613,0.625);
                        sum += GRABPIXEL(0.1594,0.688);
                        sum += GRABPIXEL(0.1575,0.750);
                        sum += GRABPIXEL(0.1556,0.813);
                        sum += GRABPIXEL(0.1538,0.875);
                        sum += GRABPIXEL(0.1519,0.938);
                        sum += GRABPIXEL(0.1500,1.000);
                        sum += GRABPIXEL(0.1481,1.063);
                        sum += GRABPIXEL(0.1463,1.125);
                        sum += GRABPIXEL(0.1444,1.188);
                        sum += GRABPIXEL(0.1425,1.250);
                        sum += GRABPIXEL(0.1406,1.313);
                        sum += GRABPIXEL(0.1388,1.375);
                        sum += GRABPIXEL(0.1369,1.438);
                        sum += GRABPIXEL(0.1350,1.500);
                        sum += GRABPIXEL(0.1331,1.563);
                        sum += GRABPIXEL(0.1313,1.625);
                        sum += GRABPIXEL(0.1294,1.688);
                        sum += GRABPIXEL(0.1275,1.750);
                        sum += GRABPIXEL(0.1256,1.813);
                        sum += GRABPIXEL(0.1238,1.875);
                        sum += GRABPIXEL(0.1219,1.938);
                        sum += GRABPIXEL(0.1200,2.000);
                        sum += GRABPIXEL(0.1181,2.063);
                        sum += GRABPIXEL(0.1163,2.125);
                        sum += GRABPIXEL(0.1144,2.188);
                        sum += GRABPIXEL(0.1125,2.250);
                        sum += GRABPIXEL(0.1106,2.313);
                        sum += GRABPIXEL(0.1088,2.375);
                        sum += GRABPIXEL(0.1069,2.438);
                        sum += GRABPIXEL(0.1050,2.500);
                        sum += GRABPIXEL(0.1031,2.563);
                        sum += GRABPIXEL(0.1013,2.625);
                        sum += GRABPIXEL(0.0994,2.688);
                        sum += GRABPIXEL(0.0975,2.750);
                        sum += GRABPIXEL(0.0956,2.813);
                        sum += GRABPIXEL(0.0938,2.875);
                        sum += GRABPIXEL(0.0919,2.938);
                        sum += GRABPIXEL(0.0900,3.000);
                        sum += GRABPIXEL(0.0875,3.063);
                        sum += GRABPIXEL(0.0850,3.125);
                        sum += GRABPIXEL(0.0825,3.188);
                        sum += GRABPIXEL(0.0800,3.250);
                        sum += GRABPIXEL(0.0775,3.313);
                        sum += GRABPIXEL(0.0750,3.375);
                        sum += GRABPIXEL(0.0725,3.438);
                        sum += GRABPIXEL(0.0700,3.500);
                        sum += GRABPIXEL(0.0675,3.563);
                        sum += GRABPIXEL(0.0650,3.625);
                        sum += GRABPIXEL(0.0625,3.688);
                        sum += GRABPIXEL(0.0600,3.750);
                        sum += GRABPIXEL(0.0575,3.813);
                        sum += GRABPIXEL(0.0550,3.875);
                        sum += GRABPIXEL(0.0525,3.938);
                        sum += GRABPIXEL(0.0500,4.000);

                        sum /= ( 15.25);
                    }

					
                    return sum;
                }
                ENDCG
            }



            Pass 
            {
                Tags { "LightMode" = "Always" }
				ZTest Always Cull Off ZWrite Off
				Fog{ Mode off }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t 
                {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f 
                {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 mask : TEXCOORD1; 
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v) 
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.mask = TRANSFORM_TEX( v.texcoord, _MainTex );
                    return o;
                }
             
                
                float _BlurSize;
                int _Quality;
                sampler2D _MainTex;
				float4 _MainTex_TexelSize;
				//float _Lightness;

                half4 frag( v2f i ) : COLOR 
                {


                 
                    half4 sum = half4(0,0,0,0);
                    #define GRABPIXEL(weight,kernelx) tex2Dproj( _MainTex, UNITY_PROJ_COORD(float4(i.uvgrab.x + _MainTex_TexelSize.x * kernelx * _BlurSize, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight


                    if (_Quality == 0)
                    {
	                    sum += GRABPIXEL(0.05000,-4.0000);
						sum += GRABPIXEL(0.09000,-3.0000);
						sum += GRABPIXEL(0.12000,-2.0000);
						sum += GRABPIXEL(0.15000,-1.0000);
						sum += GRABPIXEL(0.18000,0.0000);
						sum += GRABPIXEL(0.15000,1.0000);
						sum += GRABPIXEL(0.12000,2.0000);
						sum += GRABPIXEL(0.09000,3.0000);
						sum += GRABPIXEL(0.05000,4.0000);

						sum /= (  1 );
                    }
                    else if (_Quality == 1)
                    {
						sum += GRABPIXEL(0.0500,-4.000);
						sum += GRABPIXEL(0.0700,-3.500);
						sum += GRABPIXEL(0.0900,-3.000);
						sum += GRABPIXEL(0.1050,-2.500);
						sum += GRABPIXEL(0.1200,-2.000);
						sum += GRABPIXEL(0.1350,-1.500);
						sum += GRABPIXEL(0.1500,-1.000);
						sum += GRABPIXEL(0.1650,-0.500);
						sum += GRABPIXEL(0.1800,0.000);
						sum += GRABPIXEL(0.1650,0.500);
						sum += GRABPIXEL(0.1500,1.000);
						sum += GRABPIXEL(0.1350,1.500);
						sum += GRABPIXEL(0.1200,2.000);
						sum += GRABPIXEL(0.1050,2.500);
						sum += GRABPIXEL(0.0900,3.000);
						sum += GRABPIXEL(0.0700,3.500);
						sum += GRABPIXEL(0.0500,4.000);

						sum /= (  1.95);
                    }
                    else if (_Quality == 2)
                    {
						sum += GRABPIXEL(0.0500,-4.000);
						sum += GRABPIXEL(0.0600,-3.750);
						sum += GRABPIXEL(0.0700,-3.500);
						sum += GRABPIXEL(0.0800,-3.250);
						sum += GRABPIXEL(0.0900,-3.000);
						sum += GRABPIXEL(0.0975,-2.750);
						sum += GRABPIXEL(0.1050,-2.500);
						sum += GRABPIXEL(0.1125,-2.250);
						sum += GRABPIXEL(0.1200,-2.000);
						sum += GRABPIXEL(0.1275,-1.750);
						sum += GRABPIXEL(0.1350,-1.500);
						sum += GRABPIXEL(0.1425,-1.250);
						sum += GRABPIXEL(0.1500,-1.000);
						sum += GRABPIXEL(0.1575,-0.750);
						sum += GRABPIXEL(0.1650,-0.500);
						sum += GRABPIXEL(0.1725,-0.250);
						sum += GRABPIXEL(0.1800,0.000);
						sum += GRABPIXEL(0.1725,0.250);
						sum += GRABPIXEL(0.1650,0.500);
						sum += GRABPIXEL(0.1575,0.750);
						sum += GRABPIXEL(0.1500,1.000);
						sum += GRABPIXEL(0.1425,1.250);
						sum += GRABPIXEL(0.1350,1.500);
						sum += GRABPIXEL(0.1275,1.750);
						sum += GRABPIXEL(0.1200,2.000);
						sum += GRABPIXEL(0.1125,2.250);
						sum += GRABPIXEL(0.1050,2.500);
						sum += GRABPIXEL(0.0975,2.750);
						sum += GRABPIXEL(0.0900,3.000);
						sum += GRABPIXEL(0.0800,3.250);
						sum += GRABPIXEL(0.0700,3.500);
						sum += GRABPIXEL(0.0600,3.750);
						sum += GRABPIXEL(0.0500,4.000);

						sum /= (  3.85);
                    }
                    else if (_Quality == 3)
                    {
						sum += GRABPIXEL(0.0500,-4.000);
						sum += GRABPIXEL(0.0550,-3.875);
						sum += GRABPIXEL(0.0600,-3.750);
						sum += GRABPIXEL(0.0650,-3.625);
						sum += GRABPIXEL(0.0700,-3.500);
						sum += GRABPIXEL(0.0750,-3.375);
						sum += GRABPIXEL(0.0800,-3.250);
						sum += GRABPIXEL(0.0850,-3.125);
						sum += GRABPIXEL(0.0900,-3.000);
						sum += GRABPIXEL(0.0938,-2.875);
						sum += GRABPIXEL(0.0975,-2.750);
						sum += GRABPIXEL(0.1013,-2.625);
						sum += GRABPIXEL(0.1050,-2.500);
						sum += GRABPIXEL(0.1088,-2.375);
						sum += GRABPIXEL(0.1125,-2.250);
						sum += GRABPIXEL(0.1163,-2.125);
						sum += GRABPIXEL(0.1200,-2.000);
						sum += GRABPIXEL(0.1238,-1.875);
						sum += GRABPIXEL(0.1275,-1.750);
						sum += GRABPIXEL(0.1313,-1.625);
						sum += GRABPIXEL(0.1350,-1.500);
						sum += GRABPIXEL(0.1388,-1.375);
						sum += GRABPIXEL(0.1425,-1.250);
						sum += GRABPIXEL(0.1463,-1.125);
						sum += GRABPIXEL(0.1500,-1.000);
						sum += GRABPIXEL(0.1538,-0.875);
						sum += GRABPIXEL(0.1575,-0.750);
						sum += GRABPIXEL(0.1613,-0.625);
						sum += GRABPIXEL(0.1650,-0.500);
						sum += GRABPIXEL(0.1688,-0.375);
						sum += GRABPIXEL(0.1725,-0.250);
						sum += GRABPIXEL(0.1763,-0.125);
						sum += GRABPIXEL(0.1800,0.000);
						sum += GRABPIXEL(0.1763,0.125);
						sum += GRABPIXEL(0.1725,0.250);
						sum += GRABPIXEL(0.1688,0.375);
						sum += GRABPIXEL(0.1650,0.500);
						sum += GRABPIXEL(0.1613,0.625);
						sum += GRABPIXEL(0.1575,0.750);
						sum += GRABPIXEL(0.1538,0.875);
						sum += GRABPIXEL(0.1500,1.000);
						sum += GRABPIXEL(0.1463,1.125);
						sum += GRABPIXEL(0.1425,1.250);
						sum += GRABPIXEL(0.1388,1.375);
						sum += GRABPIXEL(0.1350,1.500);
						sum += GRABPIXEL(0.1313,1.625);
						sum += GRABPIXEL(0.1275,1.750);
						sum += GRABPIXEL(0.1238,1.875);
						sum += GRABPIXEL(0.1200,2.000);
						sum += GRABPIXEL(0.1163,2.125);
						sum += GRABPIXEL(0.1125,2.250);
						sum += GRABPIXEL(0.1088,2.375);
						sum += GRABPIXEL(0.1050,2.500);
						sum += GRABPIXEL(0.1013,2.625);
						sum += GRABPIXEL(0.0975,2.750);
						sum += GRABPIXEL(0.0938,2.875);
						sum += GRABPIXEL(0.0900,3.000);
						sum += GRABPIXEL(0.0850,3.125);
						sum += GRABPIXEL(0.0800,3.250);
						sum += GRABPIXEL(0.0750,3.375);
						sum += GRABPIXEL(0.0700,3.500);
						sum += GRABPIXEL(0.0650,3.625);
						sum += GRABPIXEL(0.0600,3.750);
						sum += GRABPIXEL(0.0550,3.875);
						sum += GRABPIXEL(0.0500,4.000);

						sum /= (  7.65);
                    }
                    else if (_Quality >= 4)
                    {

                        sum += GRABPIXEL(0.0500,-4.000);
                        sum += GRABPIXEL(0.0525,-3.938);
                        sum += GRABPIXEL(0.0550,-3.875);
                        sum += GRABPIXEL(0.0575,-3.813);
                        sum += GRABPIXEL(0.0600,-3.750);
                        sum += GRABPIXEL(0.0625,-3.688);
                        sum += GRABPIXEL(0.0650,-3.625);
                        sum += GRABPIXEL(0.0675,-3.563);
                        sum += GRABPIXEL(0.0700,-3.500);
                        sum += GRABPIXEL(0.0725,-3.438);
                        sum += GRABPIXEL(0.0750,-3.375);
                        sum += GRABPIXEL(0.0775,-3.313);
                        sum += GRABPIXEL(0.0800,-3.250);
                        sum += GRABPIXEL(0.0825,-3.188);
                        sum += GRABPIXEL(0.0850,-3.125);
                        sum += GRABPIXEL(0.0875,-3.063);
                        sum += GRABPIXEL(0.0900,-3.000);
                        sum += GRABPIXEL(0.0919,-2.938);
                        sum += GRABPIXEL(0.0938,-2.875);
                        sum += GRABPIXEL(0.0956,-2.813);
                        sum += GRABPIXEL(0.0975,-2.750);
                        sum += GRABPIXEL(0.0994,-2.688);
                        sum += GRABPIXEL(0.1013,-2.625);
                        sum += GRABPIXEL(0.1031,-2.563);
                        sum += GRABPIXEL(0.1050,-2.500);
                        sum += GRABPIXEL(0.1069,-2.438);
                        sum += GRABPIXEL(0.1088,-2.375);
                        sum += GRABPIXEL(0.1106,-2.313);
                        sum += GRABPIXEL(0.1125,-2.250);
                        sum += GRABPIXEL(0.1144,-2.188);
                        sum += GRABPIXEL(0.1163,-2.125);
                        sum += GRABPIXEL(0.1181,-2.063);
                        sum += GRABPIXEL(0.1200,-2.000);
                        sum += GRABPIXEL(0.1219,-1.938);
                        sum += GRABPIXEL(0.1238,-1.875);
                        sum += GRABPIXEL(0.1256,-1.813);
                        sum += GRABPIXEL(0.1275,-1.750);
                        sum += GRABPIXEL(0.1294,-1.688);
                        sum += GRABPIXEL(0.1313,-1.625);
                        sum += GRABPIXEL(0.1331,-1.563);
                        sum += GRABPIXEL(0.1350,-1.500);
                        sum += GRABPIXEL(0.1369,-1.438);
                        sum += GRABPIXEL(0.1388,-1.375);
                        sum += GRABPIXEL(0.1406,-1.313);
                        sum += GRABPIXEL(0.1425,-1.250);
                        sum += GRABPIXEL(0.1444,-1.188);
                        sum += GRABPIXEL(0.1463,-1.125);
                        sum += GRABPIXEL(0.1481,-1.063);
                        sum += GRABPIXEL(0.1500,-1.000);
                        sum += GRABPIXEL(0.1519,-0.938);
                        sum += GRABPIXEL(0.1538,-0.875);
                        sum += GRABPIXEL(0.1556,-0.813);
                        sum += GRABPIXEL(0.1575,-0.750);
                        sum += GRABPIXEL(0.1594,-0.688);
                        sum += GRABPIXEL(0.1613,-0.625);
                        sum += GRABPIXEL(0.1631,-0.563);
                        sum += GRABPIXEL(0.1650,-0.500);
                        sum += GRABPIXEL(0.1669,-0.438);
                        sum += GRABPIXEL(0.1688,-0.375);
                        sum += GRABPIXEL(0.1706,-0.313);
                        sum += GRABPIXEL(0.1725,-0.250);
                        sum += GRABPIXEL(0.1744,-0.188);
                        sum += GRABPIXEL(0.1763,-0.125);
                        sum += GRABPIXEL(0.1781,-0.063);
                        sum += GRABPIXEL(0.1800,0.000);
                        sum += GRABPIXEL(0.1781,0.063);
                        sum += GRABPIXEL(0.1763,0.125);
                        sum += GRABPIXEL(0.1744,0.188);
                        sum += GRABPIXEL(0.1725,0.250);
                        sum += GRABPIXEL(0.1706,0.313);
                        sum += GRABPIXEL(0.1688,0.375);
                        sum += GRABPIXEL(0.1669,0.438);
                        sum += GRABPIXEL(0.1650,0.500);
                        sum += GRABPIXEL(0.1631,0.563);
                        sum += GRABPIXEL(0.1613,0.625);
                        sum += GRABPIXEL(0.1594,0.688);
                        sum += GRABPIXEL(0.1575,0.750);
                        sum += GRABPIXEL(0.1556,0.813);
                        sum += GRABPIXEL(0.1538,0.875);
                        sum += GRABPIXEL(0.1519,0.938);
                        sum += GRABPIXEL(0.1500,1.000);
                        sum += GRABPIXEL(0.1481,1.063);
                        sum += GRABPIXEL(0.1463,1.125);
                        sum += GRABPIXEL(0.1444,1.188);
                        sum += GRABPIXEL(0.1425,1.250);
                        sum += GRABPIXEL(0.1406,1.313);
                        sum += GRABPIXEL(0.1388,1.375);
                        sum += GRABPIXEL(0.1369,1.438);
                        sum += GRABPIXEL(0.1350,1.500);
                        sum += GRABPIXEL(0.1331,1.563);
                        sum += GRABPIXEL(0.1313,1.625);
                        sum += GRABPIXEL(0.1294,1.688);
                        sum += GRABPIXEL(0.1275,1.750);
                        sum += GRABPIXEL(0.1256,1.813);
                        sum += GRABPIXEL(0.1238,1.875);
                        sum += GRABPIXEL(0.1219,1.938);
                        sum += GRABPIXEL(0.1200,2.000);
                        sum += GRABPIXEL(0.1181,2.063);
                        sum += GRABPIXEL(0.1163,2.125);
                        sum += GRABPIXEL(0.1144,2.188);
                        sum += GRABPIXEL(0.1125,2.250);
                        sum += GRABPIXEL(0.1106,2.313);
                        sum += GRABPIXEL(0.1088,2.375);
                        sum += GRABPIXEL(0.1069,2.438);
                        sum += GRABPIXEL(0.1050,2.500);
                        sum += GRABPIXEL(0.1031,2.563);
                        sum += GRABPIXEL(0.1013,2.625);
                        sum += GRABPIXEL(0.0994,2.688);
                        sum += GRABPIXEL(0.0975,2.750);
                        sum += GRABPIXEL(0.0956,2.813);
                        sum += GRABPIXEL(0.0938,2.875);
                        sum += GRABPIXEL(0.0919,2.938);
                        sum += GRABPIXEL(0.0900,3.000);
                        sum += GRABPIXEL(0.0875,3.063);
                        sum += GRABPIXEL(0.0850,3.125);
                        sum += GRABPIXEL(0.0825,3.188);
                        sum += GRABPIXEL(0.0800,3.250);
                        sum += GRABPIXEL(0.0775,3.313);
                        sum += GRABPIXEL(0.0750,3.375);
                        sum += GRABPIXEL(0.0725,3.438);
                        sum += GRABPIXEL(0.0700,3.500);
                        sum += GRABPIXEL(0.0675,3.563);
                        sum += GRABPIXEL(0.0650,3.625);
                        sum += GRABPIXEL(0.0625,3.688);
                        sum += GRABPIXEL(0.0600,3.750);
                        sum += GRABPIXEL(0.0575,3.813);
                        sum += GRABPIXEL(0.0550,3.875);
                        sum += GRABPIXEL(0.0525,3.938);
                        sum += GRABPIXEL(0.0500,4.000);

                        sum /= ( 15.25);
                    }


                    return sum;

                }
                ENDCG
            }
        }
    }
}
