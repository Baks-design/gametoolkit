using UnityEngine;
using UnityEngine.Rendering;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public readonly struct WrappedUnsafeCommandBuffer : IWrappedCommandBuffer
    {
        readonly UnsafeCommandBuffer unsafeCommandBuffer;

        public WrappedUnsafeCommandBuffer(UnsafeCommandBuffer unsafeCommandBuffer) =>
            this.unsafeCommandBuffer = unsafeCommandBuffer;

        public void SetRenderTarget(
            RenderTargetIdentifier rt,
            int mipLevel,
            CubemapFace cubemapFace,
            int depthSlice
        ) => unsafeCommandBuffer.SetRenderTarget(rt, mipLevel, cubemapFace, depthSlice);

        public void DrawProcedural(
            Matrix4x4 matrix,
            Material material,
            int shaderPass,
            MeshTopology topology,
            int vertexCount,
            int instanceCount,
            MaterialPropertyBlock properties
        ) =>
            unsafeCommandBuffer.DrawProcedural(
                matrix,
                material,
                shaderPass,
                topology,
                vertexCount,
                instanceCount,
                properties
            );
    }
}
