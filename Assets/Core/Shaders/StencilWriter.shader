Shader "Custom/StencilWriter"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry-1"}

        Pass
        {
            Blend Zero One
            ZTest Always
            ZWrite Off
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
        }
    }
}
