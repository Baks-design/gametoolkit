using System;
using System.Runtime.CompilerServices;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public static class Mathfs
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static float Eerp(float a, float b, float t) =>
            t switch
            {
                0f => a,
                1f => b,
                _ => MathF.Pow(a, 1f - t) * MathF.Pow(b, t)
            };
    }
}
