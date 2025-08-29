using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    public class UniversalBlurFeature : ScriptableRendererFeature
    {
        [Header("Blur Settings")]
        [Range(1, 12)]
        [SerializeField]
        int iterations = 4;

        [Range(1f, 10f)]
        [SerializeField]
        float downsample = 2f;

        [Tooltip("Enable mipmaps for more efficient blur")]
        [SerializeField]
        bool enableMipMaps = true;

        [SerializeField]
        float scale = 1f;

        [SerializeField]
        float offset = 1f;

        [Header("Advanced Settings")]
        [SerializeField]
        ScaleBlurWith scaleBlurWith = ScaleBlurWith.ScreenHeight;

        [SerializeField]
        float scaleReferenceSize = 1080f;

        [Space]
        [SerializeField]
        BlurType blurType;

        [Tooltip(
            "For Overlay Canvas: AfterRenderingPostProcessing"
                + "\n\nOther: BeforeRenderingTransparents (will hide transparents)"
        )]
        [SerializeField]
        RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

        float intensity = 1f;

        [SerializeField]
        [HideInInspector]
        [Reload("Shaders/Blur.shader")]
        Shader shader;

        Material material;
        UniversalBlurPass blurPass;
        float renderScale;

        // Avoid changing intensity value, but useful for transitions
        public float Intensity
        {
            get => intensity;
            set => intensity = Mathf.Clamp(value, 0f, 1f);
        }

        /// <inheritdoc/>
        public override void Create() => blurPass = new() { renderPassEvent = injectionPoint };

        /// <inheritdoc/>
        public override void OnCameraPreCull(ScriptableRenderer renderer, in CameraData cameraData)
        {
            base.OnCameraPreCull(renderer, in cameraData);
            renderScale = cameraData.renderScale;
        }

        /// <inheritdoc/>
        public override void AddRenderPasses(
            ScriptableRenderer renderer,
            ref RenderingData renderingData
        )
        {
            if (!TrySetShadersAndMaterials())
            {
                Logging.LogErrorFormat(
                    "{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.",
                    GetType().Name,
                    name
                );
                return;
            }

            // Important to halt rendering here if camera is different,
            // otherwise render textures will detect descriptor changes
            if (
                renderingData.cameraData.isPreviewCamera
                || renderingData.cameraData.isSceneViewCamera
            )
            {
                blurPass.DrawDefaultTexture();
                return;
            }

            var passData = GetBlurConfig(renderingData);
            blurPass.Setup(passData);
            renderer.EnqueuePass(blurPass);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            blurPass?.Dispose();
            CoreUtils.Destroy(material);
        }

        bool TrySetShadersAndMaterials()
        {
            if (shader == null)
                shader = Shader.Find("Unify/Internal/Blur");

            if (material == null && shader != null)
                material = CoreUtils.CreateEngineMaterial(shader);

            return material != null;
        }

        BlurConfig GetBlurConfig(in RenderingData renderingData)
        {
            var (width, height) = GetTargetResolution(renderingData);
            return new BlurConfig
            {
                Scale = CalculateScale(),
                Width = width,
                Height = height,
                Material = material,
                Intensity = intensity,
                Downsample = downsample,
                Offset = offset,
                BlurType = blurType,
                Iterations = iterations,
                EnableMipMaps = enableMipMaps
            };
        }

        (int width, int height) GetTargetResolution(in RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            var width = Mathf.RoundToInt(descriptor.width / downsample);
            var height = Mathf.RoundToInt(descriptor.height / downsample);
            return (width, height);
        }

        float CalculateScale() =>
            scaleBlurWith switch
            {
                ScaleBlurWith.ScreenHeight
                    => scale * (Screen.height / scaleReferenceSize) * renderScale,
                ScaleBlurWith.ScreenWidth
                    => scale * (Screen.width / scaleReferenceSize) * renderScale,
                _ => scale
            };
    }
}
