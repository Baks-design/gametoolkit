using UnityEngine;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public struct LegacyPassData : IPassData
    {
        public Texture ColorSource;
        public Texture Source;
        public Texture Destination;
        public MaterialPropertyBlock MaterialPropertyBlock;
        public BlurConfig BlurConfig;

        public readonly Texture GetColorSource() => ColorSource;

        public readonly Texture GetSource() => Source;

        public readonly Texture GetDestination() => Destination;

        public readonly MaterialPropertyBlock GetMaterialPropertyBlock() => MaterialPropertyBlock;

        public readonly BlurConfig GetBlurConfig() => BlurConfig;
    }
}
