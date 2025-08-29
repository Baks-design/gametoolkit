using System;
using UnityEngine;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Outline
{
    [Serializable]
    public class OutlineSettings
    {
        public Material outlineMaterial;
        public float normalThreshold = 0.2f;
        public float outlineSize = 3.5f;
        public Color outlineColor = new(0f, 0f, 0f, 1f);
    }
}
