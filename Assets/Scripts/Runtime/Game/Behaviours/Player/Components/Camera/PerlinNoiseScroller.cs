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
            noiseOffset.x = Random.Range(0f, rand);
            noiseOffset.y = Random.Range(0f, rand);
            noiseOffset.z = Random.Range(0f, rand);
        }

        public void UpdateNoise(float deltaTime)
        {
            var scrollOffset = deltaTime * data.Frequency;

            noiseOffset.x += scrollOffset;
            noiseOffset.y += scrollOffset;
            noiseOffset.z += scrollOffset;

            cameraData.Noise.x = Mathf.PerlinNoise(noiseOffset.x, 0f);
            cameraData.Noise.y = Mathf.PerlinNoise(noiseOffset.x, 1f);
            cameraData.Noise.z = Mathf.PerlinNoise(noiseOffset.x, 2f);

            cameraData.Noise -= Vector3.one * 0.5f;
            cameraData.Noise *= data.Amplitude;
        }
    }
}
