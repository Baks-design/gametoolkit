using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.Renders.PoorGI
{
    public class PoorGIFeature : ScriptableRendererFeature
    {
        [SerializeField, Range(0, 4)]
        int UpscaleType;
        public Material SSGIMaterial;
        PoorGIPass pass;

        public override void Create()
        {
            pass = new PoorGIPass(SSGIMaterial)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingDeferredLights
            };
            pass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
        }

        public override void AddRenderPasses(
            ScriptableRenderer renderer,
            ref RenderingData renderingData
        )
        {
            if (renderingData.cameraData.isPreviewCamera)
                return;
            pass.Setup(UpscaleType);
            renderer.EnqueuePass(pass);
        }
    }
}
