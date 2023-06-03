Shader "Tree Viewer/Base"
{
        Properties {
            _MainTex ("Texture", 2D) = "white" {}
            _Color ("Main Color", COLOR) = (1,1,1,1)
        }
        SubShader {
            Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
            ZWrite Off
            Lighting Off
            Fog { Mode Off }

            Blend SrcAlpha OneMinusSrcAlpha 

            Pass {
                Color [_Color]
            }
        }
}