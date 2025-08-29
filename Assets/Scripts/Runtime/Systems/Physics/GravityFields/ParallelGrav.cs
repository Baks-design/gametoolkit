using UnityEngine;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    /// Sets the player gravity to the direction of a single vector.
    /// Useful for planes, cubes, or wall-running sequences!
    /// </summary>
    public class ParallelGrav : MonoBehaviour
    {
        [SerializeField]
        Vector3 gravVector;

        [SerializeField]
        int priority = 0;

        void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<CustomGravityAffected>(out var gravityReceiver))
                return;
            var gravPackageToSend = new GravPackageStruct(gravVector, priority);
            gravityReceiver.AddGravStruct(gravPackageToSend);
        }
    }
}
