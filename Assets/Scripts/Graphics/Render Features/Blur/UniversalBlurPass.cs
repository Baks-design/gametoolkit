using System;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    internal class UniversalBlurPass : ScriptableRenderPass, IDisposable
    {
        const string kPassName = "Universal Blur";
        const string kBlurTextureSourceName = kPassName + " - Blur Source";
        const string kBlurTextureDestinationName = kPassName + " - Blur Destination";
        readonly ProfilingSampler _profilingSampler;
        readonly MaterialPropertyBlock propertyBlock;
        BlurConfig blurConfig;

        public UniversalBlurPass()
        {
            _profilingSampler = new(kPassName);
            propertyBlock = new();
        }

        public void Setup(BlurConfig blurConfig) => this.blurConfig = blurConfig;

        public void Dispose() { }

        public void DrawDefaultTexture() =>
            // For better preview experience in editor, we just use a gray texture
            Shader.SetGlobalTexture(
                Constants.GlobalFullScreenBlurTextureId,
                Texture2D.linearGrayTexture
            );

        RenderTextureDescriptor GetDescriptor() =>
            new(blurConfig.Width, blurConfig.Height, GraphicsFormat.B10G11R11_UFloatPack32, 0)
            {
                useMipMap = blurConfig.EnableMipMaps,
                autoGenerateMips = blurConfig.EnableMipMaps
            };

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
            {
                Logging.LogError(
                    $"Skipping render pass. UniversalBlurPass requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input."
                );
                return;
            }

            var cameraColorSource = resourceData.activeColorTexture;

            var descriptor = new TextureDesc(GetDescriptor()) { name = kBlurTextureSourceName };
            descriptor.name = kBlurTextureDestinationName;

            var source = renderGraph.CreateTexture(descriptor);
            var destination = renderGraph.CreateTexture(descriptor);

            using var builder = renderGraph.AddUnsafePass<RenderGraphPassData>(
                kPassName,
                out var passData,
                _profilingSampler
            );

            passData.ColorSource = cameraColorSource;
            passData.Source = source;
            passData.Destination = destination;
            passData.MaterialPropertyBlock = propertyBlock;
            passData.BlurConfig = blurConfig;

            builder.AllowPassCulling(false);
            builder.UseTexture(source, AccessFlags.ReadWrite);
            builder.UseTexture(destination, AccessFlags.ReadWrite);
            builder.SetGlobalTextureAfterPass(destination, Constants.GlobalFullScreenBlurTextureId);
            builder.SetRenderFunc<RenderGraphPassData>(
                (data, ctx) =>
                {
                    BlurPasses.KawaseExecutePass(data, new WrappedUnsafeCommandBuffer(ctx.cmd));
                }
            );
        }
    }
}
