#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameToolkit.Runtime.Graphics.Renders.PoorGI
{
    public class DomainReloadHook
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#pragma warning disable UDR0001 // Domain Reload Analyzer

        static void InitializeOnLoad() =>
            AssemblyReloadEvents.beforeAssemblyReload += PoorGIPass.CleanUp;
#pragma warning restore UDR0001 // Domain Reload Analyzer

#endif
    }
}
