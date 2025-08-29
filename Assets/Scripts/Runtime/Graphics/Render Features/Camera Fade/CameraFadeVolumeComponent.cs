using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.CameraFade
{
    /// <summary>
    /// Volume component for the camera fade.
    /// </summary>
    [VolumeComponentMenu("Custom/Camera Fade")]
    [VolumeRequiresRendererFeatures(typeof(CameraFadeRendererFeature))]
    public sealed class CameraFadeVolumeComponent : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter progress = new(0f, 0f, 1f);
        ColorParameter color = new(UnityEngine.Color.black, false, false, false, false);

        public ColorParameter Color
        {
            get => color;
            set => color = value;
        }

        public CameraFadeVolumeComponent()
            : base() => displayName = "Camera Fade";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public bool IsActive() => progress.value > 0f;
    }
}
