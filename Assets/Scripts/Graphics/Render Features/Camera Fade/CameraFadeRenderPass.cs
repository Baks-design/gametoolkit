using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.CameraFade
{
    /// <summary>
    /// The camera fade render pass.
    /// </summary>
    public sealed class CameraFadeRenderPass : ScriptableRenderPass
    {
        /// <summary>
        /// Holds the data needed by the execution of the camera fade render pass subpasses.
        /// </summary>
        class PassData
        {
            public TextureHandle target;
            public TextureHandle source;
            public Material material;
            public int materialPassIndex;
            public TextureHandle blitTextureHandle;
        }

        static readonly int ColorId = Shader.PropertyToID("_Color");
        static readonly int ProgressId = Shader.PropertyToID("_Progress");
        readonly Material cameraFadeMaterial;
        readonly RTHandle blitRtHandle;

        public CameraFadeRenderPass(Material cameraFadeMaterial)
            : base()
        {
            this.cameraFadeMaterial = cameraFadeMaterial;
            profilingSampler = new ProfilingSampler("Camera Fade");
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            requiresIntermediateTexture = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="renderGraph"></param>
        /// <param name="frameData"></param>
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            CreateRenderGraphTextures(renderGraph, cameraData, out var blitTextureHandle);

            using (
                var builder = renderGraph.AddRasterRenderPass(
                    "Camera Fade Pass",
                    out PassData passData,
                    profilingSampler
                )
            )
            {
                passData.source = resourceData.cameraColor;
                passData.target = blitTextureHandle;
                passData.material = cameraFadeMaterial;
                passData.materialPassIndex = 0;

                builder.SetRenderAttachment(blitTextureHandle, 0, AccessFlags.WriteAll);
                builder.UseTexture(resourceData.cameraColor);
                builder.SetRenderFunc(
                    (PassData data, RasterGraphContext context) => ExecutePass(data, context)
                );
            }

            resourceData.cameraColor = blitTextureHandle;
        }

        /// <summary>
        /// Creates and returns all the necessary render graph textures.
        /// </summary>
        /// <param name="renderGraph"></param>
        /// <param name="cameraData"></param>
        /// <param name="blitTextureHandle"></param>
        void CreateRenderGraphTextures(
            RenderGraph renderGraph,
            UniversalCameraData cameraData,
            out TextureHandle blitTextureHandle
        )
        {
            var cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
            cameraTargetDescriptor.depthBufferBits = (int)DepthBits.None;

            blitTextureHandle = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                cameraTargetDescriptor,
                "_CameraFade",
                false
            );
        }

        /// <summary>
        /// Executes the pass with the information from the pass data.
        /// </summary>
        /// <param name="passData"></param>
        /// <param name="context"></param>
        static void ExecutePass(PassData passData, RasterGraphContext context)
        {
            var cameraFadeVolume =
                VolumeManager.instance.stack.GetComponent<CameraFadeVolumeComponent>();

            var cameraFadeMaterial = passData.material;
            cameraFadeMaterial.SetColor(ColorId, cameraFadeVolume.Color.value);
            cameraFadeMaterial.SetFloat(ProgressId, cameraFadeVolume.progress.value);

            Blitter.BlitTexture(
                context.cmd,
                passData.source,
                Vector2.one,
                passData.material,
                passData.materialPassIndex
            );
        }

        /// <summary>
        /// Disposes the resources used by this pass.
        /// </summary>
        public void Dispose() => blitRtHandle?.Release();
    }
}
