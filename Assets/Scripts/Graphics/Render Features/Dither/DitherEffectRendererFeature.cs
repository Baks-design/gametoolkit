using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Dither
{
    public class DitherEffectRendererFeature : ScriptableRendererFeature
    {
        [Tooltip("The material used when making the blit operation.")]
        public Material material;

        [Tooltip("The event where to inject the pass.")]
        public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

        DitherEffectPass pass;

        public override void Create() =>
            pass = new DitherEffectPass { renderPassEvent = injectionPoint };

        public override void AddRenderPasses(
            ScriptableRenderer renderer,
            ref RenderingData renderingData
        )
        {
            if (material == null)
            {
                Logging.LogWarning(
                    "DitherEffectRendererFeature material is null and will be skipped."
                );
                return;
            }

            pass.Setup(material);
            renderer.EnqueuePass(pass);
        }
    }
}
