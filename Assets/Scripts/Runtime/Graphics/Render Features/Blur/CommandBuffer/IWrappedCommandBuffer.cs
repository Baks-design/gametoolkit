using UnityEngine;
using UnityEngine.Rendering;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public interface IWrappedCommandBuffer
    {
        void SetRenderTarget(
            RenderTargetIdentifier rt,
            int mipLevel,
            CubemapFace cubemapFace,
            int depthSlice
        );
        void DrawProcedural(
            Matrix4x4 matrix,
            Material material,
            int shaderPass,
            MeshTopology topology,
            int vertexCount,
            int instanceCount,
            MaterialPropertyBlock properties
        );
    }
}
