using UnityEngine;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public interface IPassData
    {
        public BlurConfig GetBlurConfig();
        public MaterialPropertyBlock GetMaterialPropertyBlock();
        public Texture GetColorSource();
        public Texture GetSource();
        public Texture GetDestination();
    }
}
