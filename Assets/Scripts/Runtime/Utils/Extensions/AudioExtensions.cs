using UnityEngine;

namespace GameToolkit.Runtime.Utils.Extensions
{
    public static class AudioExtensions
    {
        /// <summary>
        /// Converts a float value representing a volume slider position into a logarithmic volume,
        /// giving us a smoother and more natural-sounding progression when a volume slider is moved.
        /// The math here performs the following steps:
        /// - Ensures the slider value is equal to or greater than 0.0001 to avoid passing 0 to the logarithm function.
        /// - Takes the base-10 logarithm of the slider value.
        /// - Multiplies the result by 20. In audio engineering, a change of 1 unit in a dB scale is approximately equivalent
        ///   to what the human ear perceives as a doubling or halving of the volume, hence the multiplication by 20.
        ///
        /// /// This method is useful for normalizing UI Volume Sliders used with Unity's Audio Mixer.
        /// </summary>
        public static float ToLogarithmicVolume(this float sliderValue) =>
            Mathf.Log10(sliderValue * 0.9999f + 0.0001f) * 20f;

        /// <summary>
        /// Converts a linear fraction [0, 1] to a logarithmic scale [0, 1] that mimics human perception of sound volume.
        /// Human hearing perceives volume logarithmically, so this provides more natural audio fading effects.
        /// </summary>
        /// <param name="fraction">Linear input value in range [0, 1]</param>
        /// <returns>Logarithmically scaled value in range [0, 1]</returns>
        public static float ToLogarithmicFraction(this float fraction) =>
            Mathf.Log10(1f + 9f * fraction);
    }
}
