using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Dither
{
    public class DitherEffectPass : ScriptableRenderPass
    {
        const string _passName = "DitherEffectPass";
        Material blitMaterial;

        public void Setup(Material blitMaterial)
        {
            this.blitMaterial = blitMaterial;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var stack = VolumeManager.instance.stack;
            // var customEffect = stack.GetComponent<SphereVolumeComponent>(); TODO:
            // if (!customEffect.IsActive())
            //     return;

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
            {
                Logging.LogError(
                    $"Skipping render pass. DitherEffectRendererFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input."
                );
                return;
            }

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{_passName}";
            destinationDesc.clearBuffer = false;

            var destination = renderGraph.CreateTexture(destinationDesc);

            var para = new RenderGraphUtils.BlitMaterialParameters(
                source,
                destination,
                blitMaterial,
                0
            );

            renderGraph.AddBlitPass(para, _passName);

            resourceData.cameraColor = destination;
        }
    }
}
