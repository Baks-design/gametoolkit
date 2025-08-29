using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Outline
{
    public class OutlinePass : ScriptableRenderPass
    {
        class OutlinePassData
        {
            internal Material blitMaterial;
        }

        const string OUTLINE_PASS_NAME = "OutlinePass";
        static readonly int normalThreshold = Shader.PropertyToID("_NormalThreshold");
        static readonly int outlineColor = Shader.PropertyToID("_OutlineColor");
        static readonly int outlineSize = Shader.PropertyToID("_OutlineSize");
        readonly OutlineSettings settings;

        public OutlinePass(OutlineSettings settings) => this.settings = settings;

        void UpdateOutlineSettings()
        {
            settings.outlineMaterial.SetFloat(normalThreshold, settings.normalThreshold);
            settings.outlineMaterial.SetColor(outlineColor, settings.outlineColor);
            settings.outlineMaterial.SetFloat(outlineSize, settings.outlineSize);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var renderingData = frameData.Get<UniversalRenderingData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            var cameraTexHandle = resourceData.activeColorTexture;

            if (settings == null || settings.outlineMaterial == null)
                return;

            UpdateOutlineSettings();

            if (!cameraTexHandle.IsValid())
            {
                Logging.LogWarning("OutlinePass: Invalid source or destination texture.");
                return;
            }

            using var builder = renderGraph.AddRasterRenderPass<OutlinePassData>(
                OUTLINE_PASS_NAME,
                out var passData
            );
            passData.blitMaterial = settings.outlineMaterial;

            builder.SetRenderAttachment(cameraTexHandle, 0);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc(
                (OutlinePassData data, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, new Vector4(1f, 1f, 0f, 0f), data.blitMaterial, 0);
                }
            );
        }
    }
}
