Shader "Custom/CameraTargetPatch" {
 
    Properties {
 
        _MainTex("Texture(RGB)", 2D) = "white" { }   
    }
     
    SubShader{  
     
    Pass{
     
    Cull Back
     
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
     
    #include "UnityCG.cginc"
     
    sampler2D _MainTex;
    float4x4 _MATRIX_MVP;
    float _yScale;
    float _xScale;
 
    struct v2f {
        float4  pos : SV_POSITION;
        float2  uv : TEXCOORD0;
    };
 
    v2f vert(appdata_base v){
         
        v2f o;
        float4 uvTmp;
 
        // calculate new uv in camera image
        uvTmp = mul(_MATRIX_MVP, float4(v.texcoord.x-0.5f,v.texcoord.y-0.5f,0,1));
        uvTmp.x = uvTmp.x/uvTmp.w;
        uvTmp.y = uvTmp.y/uvTmp.w;
        uvTmp.z = uvTmp.z/uvTmp.w;
 
        // some swap for different coordinate system
        uvTmp.x = (uvTmp.x + 1.0f)/2.0f;
        uvTmp.y = (uvTmp.y + 1.0f)/2.0f;
        o.uv.x = ((1-uvTmp.y)-0.5f)*_xScale+0.5f;
        o.uv.y = ((1-uvTmp.x)-0.5f)*_yScale+0.5f;
 
        //The position of the vertex should not be frozen, so use 
        //the standard UNITY_MATRIX_MVP matrix for that.
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 
        return o;
    }
     
    half4 frag(v2f i) : COLOR{
        half4 texcol = tex2D(_MainTex, i.uv);
        return texcol;
    }
     
    ENDCG
     
    }
    }
}