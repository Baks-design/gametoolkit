using System.Buffers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace GameToolkit.Runtime.Graphics.Renders.PoorGI
{
    public class PoorGIPass : ScriptableRenderPass
    {
        readonly Material ssgiMaterial;
        int upscaleType;
#pragma warning disable UDR0001 // Domain Reload Analyzer
        static Mesh triangleMesh;
#pragma warning restore UDR0001 // Domain Reload Analyzer

        public PoorGIPass(Material ssgiMaterial)
        {
            this.ssgiMaterial = ssgiMaterial;
            CreateFullScreenTriangle();
        }

        public void Setup(int upscaleType) => this.upscaleType = upscaleType;

        class PassData
        {
            public TextureHandle CameraDepth;
            public int TraceWidth;
            public int TraceHeight;
            public TextureHandle TraceDepth;
            public TextureHandle VarianceDepth;
            public TextureHandle GIBuffer;
            public TextureHandle TempTraceBuffer;
            public TextureHandle SHBuffer;
            public TextureHandle CameraColorTarget;
            public TextureHandle GBuffer0;
            public Material SSGIMaterial;
            public int UpsaleType;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            using var builder = renderGraph.AddUnsafePass<PassData>(
                nameof(PoorGIPass),
                out var passData
            );
            builder.AllowPassCulling(false);

            passData.UpsaleType = upscaleType;
            passData.SSGIMaterial = ssgiMaterial;

            passData.CameraDepth = resourceData.cameraDepthTexture;
            builder.UseTexture(passData.CameraDepth);

            passData.CameraColorTarget = resourceData.activeColorTexture;
            builder.UseTexture(passData.CameraColorTarget);

            var screenWidth = cameraData.scaledWidth;
            var screenHeight = cameraData.scaledHeight;

            var traceScale = 4f;
            // BUG: If frame buffer is not divisible by 4, border appears on right or top side of MaxDepth.
            // TODO: Make bigger buffer for MaxDepth, but trace only valid pixels.
            var traceWidth = Mathf.FloorToInt(screenWidth / traceScale);
            var traceHeight = Mathf.FloorToInt(screenHeight / traceScale);
            var traceBufferWidth = Mathf.CeilToInt(screenWidth / traceScale);
            var traceBufferHeight = Mathf.CeilToInt(screenHeight / traceScale);

            passData.TraceWidth = traceWidth;
            passData.TraceHeight = traceHeight;

            var traceDepthDesc = new TextureDesc(traceBufferWidth, traceBufferHeight)
            {
                name = "TraceDepth",
                format = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.RFloat, false),
            };
            passData.TraceDepth = builder.CreateTransientTexture(traceDepthDesc);

            // NOTE: Disable variance depth for now.
            // var varianceDepthDesc = new TextureDesc(traceBufferWidth, traceBufferHeight)
            // {
            //     name = "VarianceDepth",
            //     format = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.RGHalf, false),
            // };
            // passData.VarianceDepth = builder.CreateTransientTexture(varianceDepthDesc);

            var giBufferDesc = new TextureDesc(traceBufferWidth, traceBufferHeight)
            {
                name = "IrradianceBuffer",
                filterMode = FilterMode.Bilinear,
                format = GraphicsFormatUtility.GetGraphicsFormat(
                    RenderTextureFormat.ARGBFloat,
                    isSRGB: false
                ),
                clearBuffer = false,
            };
            passData.GIBuffer = renderGraph.CreateTexture(giBufferDesc);
            builder.UseTexture(passData.GIBuffer);

            giBufferDesc.name = "SHBuffer";
            passData.SHBuffer = builder.CreateTransientTexture(giBufferDesc);

            giBufferDesc.name = "Temp";
            passData.TempTraceBuffer = builder.CreateTransientTexture(giBufferDesc);

            passData.GBuffer0 = resourceData.gBuffer[0];
            builder.UseTexture(passData.GBuffer0);

            builder.SetRenderFunc<PassData>(
                static (data, context) =>
                {
                    const int DownSampleDepthPass = 0;
                    const int TracePass = 1;
                    const int BlurHorizontalPass = 2;
                    const int BlurVerticalPass = 3;
                    const int BilateralUpsamplePass = 4;
                    //const int VarianceDepthPass = 5;
                    const int BlitBlur = 6;

                    var cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                    // Downsample Depth
                    cmd.Blit(
                        data.CameraDepth,
                        data.TraceDepth,
                        data.SSGIMaterial,
                        DownSampleDepthPass
                    );

                    // Variance Depth
                    // cmd.Blit(data.CameraDepth, data.VarianceDepth, data.SSGIMaterial, VarianceDepthPass);

                    // Downsample Color
                    cmd.Blit(
                        data.CameraColorTarget,
                        data.TempTraceBuffer,
                        data.SSGIMaterial,
                        BlitBlur
                    );
                    // Tracing
                    {
                        var bindings = CreateMRTBinding(data.GIBuffer, data.SHBuffer);
                        cmd.SetRenderTarget(bindings);

                        // TODO: Pass With MaterialPropBlock.
                        cmd.SetGlobalTexture("TraceColor", data.TempTraceBuffer);
                        cmd.SetGlobalTexture("TraceDepth", data.TraceDepth);
                        // cmd.SetGlobalTexture("VarianceDepth", data.VarianceDepth);

                        cmd.DrawMesh(
                            triangleMesh,
                            Matrix4x4.identity,
                            data.SSGIMaterial,
                            0,
                            TracePass
                        );
                    }
                    // Blur GI
                    {
                        cmd.SetGlobalTexture("RefrenceDepth", data.TraceDepth);
                        cmd.Blit(
                            data.GIBuffer,
                            data.TempTraceBuffer,
                            data.SSGIMaterial,
                            BlurHorizontalPass
                        );
                        cmd.Blit(
                            data.TempTraceBuffer,
                            data.GIBuffer,
                            data.SSGIMaterial,
                            BlurVerticalPass
                        );
                        cmd.Blit(
                            data.SHBuffer,
                            data.TempTraceBuffer,
                            data.SSGIMaterial,
                            BlurHorizontalPass
                        );
                        cmd.Blit(
                            data.TempTraceBuffer,
                            data.SHBuffer,
                            data.SSGIMaterial,
                            BlurVerticalPass
                        );
                    }

                    // Upscaling
                    cmd.SetGlobalInteger("UpscaleType", data.UpsaleType);
                    cmd.SetGlobalVector(
                        "TraceSize",
                        new Vector4(data.TraceWidth, data.TraceHeight)
                    );
                    cmd.SetGlobalTexture("TraceDepth", data.TraceDepth);
                    cmd.SetGlobalTexture("SHBuffer", data.SHBuffer);
                    cmd.SetGlobalTexture("GBuffer0", data.GBuffer0);
                    cmd.Blit(
                        data.GIBuffer,
                        data.CameraColorTarget,
                        data.SSGIMaterial,
                        BilateralUpsamplePass
                    );
                }
            );
        }

        static RenderTargetBinding CreateMRTBinding(TextureHandle colorA, TextureHandle colorB)
        {
            var targets = ArrayPool<RenderTargetIdentifier>.Shared.Rent(2);
            var load = ArrayPool<RenderBufferLoadAction>.Shared.Rent(2);
            var store = ArrayPool<RenderBufferStoreAction>.Shared.Rent(2);

            targets[0] = colorA;
            load[0] = RenderBufferLoadAction.DontCare;
            store[0] = RenderBufferStoreAction.Store;

            targets[1] = colorB;
            load[1] = RenderBufferLoadAction.DontCare;
            store[1] = RenderBufferStoreAction.Store;

            var bindings = new RenderTargetBinding()
            {
                colorRenderTargets = targets[..2],
                colorLoadActions = load[..2],
                colorStoreActions = store[..2],
                depthRenderTarget = colorA,
                flags = RenderTargetFlags.None,
            };

            ArrayPool<RenderTargetIdentifier>.Shared.Return(targets);
            ArrayPool<RenderBufferLoadAction>.Shared.Return(load);
            ArrayPool<RenderBufferStoreAction>.Shared.Return(store);

            return bindings;
        }

        static void CreateFullScreenTriangle()
        {
            /*UNITYNEARCLIPVALUE*/
            var nearClipZ = SystemInfo.usesReversedZBuffer ? 1 : -1;
            if (!triangleMesh)
                triangleMesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave,
                    vertices = GetFullScreenTriangleVertexPosition(nearClipZ),
                    uv = GetFullScreenTriangleTexCoord(),
                    triangles = new int[3] { 0, 1, 2 }
                };
        }

        // Should match Common.hlsl
        public static Vector3[] GetFullScreenTriangleVertexPosition(
            float z /*= UNITYNEARCLIPVALUE*/
        )
        {
            var r = new Vector3[3];
            for (var i = 0; i < 3; i++)
            {
                var uv = new Vector2((i << 1) & 2, i & 2);
                r[i] = new Vector3(uv.x * 2f - 1f, uv.y * 2f - 1f, z);
            }
            return r;
        }

        // Should match Common.hlsl
        public static Vector2[] GetFullScreenTriangleTexCoord()
        {
            var r = new Vector2[3];
            for (var i = 0; i < 3; i++)
            {
                if (SystemInfo.graphicsUVStartsAtTop)
                    r[i] = new Vector2((i << 1) & 2, 1.0f - (i & 2));
                else
                    r[i] = new Vector2((i << 1) & 2, i & 2);
            }
            return r;
        }

        public static void CleanUp()
        {
            if (triangleMesh)
                Object.DestroyImmediate(triangleMesh);
        }
    }
}
