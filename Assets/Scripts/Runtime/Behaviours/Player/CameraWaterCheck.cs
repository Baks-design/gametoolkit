using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class CameraWaterCheck : MonoBehaviour
    {
        readonly List<Collider> triggers = new();

        void OnTriggerEnter(Collider other)
        {
            if (!triggers.Contains(other))
                triggers.Add(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (triggers.Contains(other))
                triggers.Remove(other);
        }

        public bool IsUnderwater()
        {
            foreach (Collider trigger in triggers)
                if (trigger.GetComponentInParent<Water>())
                    return true;
            return false;
        }
    }
}
