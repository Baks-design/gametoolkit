using System;
using UnityEngine;

namespace GameToolkit.Runtime.Graphics.RenderFeatures.Blur
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowAsPass : PropertyAttribute
    {
        public string TargetMaterialField { get; private set; }

        public ShowAsPass(string targetMaterialField) => TargetMaterialField = targetMaterialField;
    }
}
