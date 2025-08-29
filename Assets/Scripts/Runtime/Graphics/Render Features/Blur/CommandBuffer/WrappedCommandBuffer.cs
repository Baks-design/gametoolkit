using UnityEngine;
using UnityEngine.Rendering;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public readonly struct WrappedCommandBuffer : IWrappedCommandBuffer
    {
        readonly CommandBuffer commandBuffer;

        public WrappedCommandBuffer(CommandBuffer commandBuffer) =>
            this.commandBuffer = commandBuffer;

        public void SetRenderTarget(
            RenderTargetIdentifier rt,
            int mipLevel,
            CubemapFace cubemapFace,
            int depthSlice
        ) => commandBuffer.SetRenderTarget(rt, mipLevel, cubemapFace, depthSlice);

        public void DrawProcedural(
            Matrix4x4 matrix,
            Material material,
            int shaderPass,
            MeshTopology topology,
            int vertexCount,
            int instanceCount,
            MaterialPropertyBlock properties
        ) =>
            commandBuffer.DrawProcedural(
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
