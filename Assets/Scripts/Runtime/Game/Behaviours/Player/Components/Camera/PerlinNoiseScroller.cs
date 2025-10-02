using UnityEngine;
using Random = UnityEngine.Random;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PerlinNoiseScroller
    {
        readonly PerlinNoiseData data;
        readonly PlayerCameraData cameraData;
        Vector3 noiseOffset;

        public PerlinNoiseScroller(PerlinNoiseData data, PlayerCameraData cameraData)
        {
            this.data = data;
            this.cameraData = cameraData;

            var rand = 32f;
            noiseOffset = new Vector3(
                Random.Range(0f, rand),
                Random.Range(0f, rand),
                Random.Range(0f, rand)
            );
        }

        public void UpdateNoise(float deltaTime)
        {
            var scrollOffset = deltaTime * data.Frequency;
            noiseOffset.x += scrollOffset;
            var noise = new Vector3(
                Mathf.PerlinNoise(noiseOffset.x, 0f),
                Mathf.PerlinNoise(noiseOffset.x, 1f),
                Mathf.PerlinNoise(noiseOffset.x, 2f)
            );
            noise = (noise - Vector3.one * 0.5f) * data.Amplitude;
            cameraData.Noise = noise;
        }
    }
}
