using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public class RenderGraphPassData : IPassData
    {
        public TextureHandle ColorSource;
        public TextureHandle Source;
        public TextureHandle Destination;
        public MaterialPropertyBlock MaterialPropertyBlock;
        public BlurConfig BlurConfig;

        public Texture GetColorSource() => ColorSource;

        public Texture GetSource() => Source;

        public Texture GetDestination() => Destination;

        public MaterialPropertyBlock GetMaterialPropertyBlock() => MaterialPropertyBlock;

        public BlurConfig GetBlurConfig() => BlurConfig;
    }
}
