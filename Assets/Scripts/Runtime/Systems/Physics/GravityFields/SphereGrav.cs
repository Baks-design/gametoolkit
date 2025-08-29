using UnityEngine;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    /// Pulls the player towards a central point."
    /// Useful for spherical plants or the ends of capsule-shaped ones.
    /// Note that centerPosition is relative to to this transform's position.
    /// In other words <0, 0, 0> would use the transform as the center.
    /// </summary>
    public class SphereGrav : MonoBehaviour
    {
        [SerializeField]
        Vector3 centerPosition = Vector3.zero;

        [SerializeField]
        float gravIntensity = 30f;

        [SerializeField]
        int priority = 0;

        void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<CustomGravityAffected>(out var gravityReceiver))
                return;

            //Calculate the Vector pointing from the player (or other gravity affected object) to the center
            var pointAtCenter =
                -1f
                * gravIntensity
                * (other.transform.position - (centerPosition + transform.position)).normalized;

            //Send gravity data to the player
            var gravPackageToSend = new GravPackageStruct(pointAtCenter, priority);
            gravityReceiver.AddGravStruct(gravPackageToSend);
        }
    }
}
