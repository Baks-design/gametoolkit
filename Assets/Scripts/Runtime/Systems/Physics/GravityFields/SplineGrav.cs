using GameToolkit.Runtime.Utils.Helpers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    /// Pulls the player towards the point on the spline closest to them!
    /// Cool for drawing out custom shapes, cylinders, and Capsules.
    /// If you make the gravIntensity negative,
    /// it works great for the inside of cylinders and quarter/half pipes.
    /// </summary>
    [RequireComponent(typeof(SplineContainer))]
    public class SplineGrav : MonoBehaviour
    {
        [Header("Spline Objects")]
        [SerializeField]
        SplineContainer splineToUse;

        [SerializeField]
        Transform nearestPointOnSpline;

        [Header("Gravity Fields")]
        [SerializeField]
        float gravIntensity = 30;

        [SerializeField]
        int priority = 0;

        void Start()
        {
            if (splineToUse == null)
                TryGetComponent(out splineToUse);

            if (nearestPointOnSpline == null)
                Logging.LogError("Nearest Point GameObject is null");
        }

        void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<CustomGravityAffected>(out var gravityReceiver))
                return;

            var nearest = new float4(0f, 0f, 0f, float.PositiveInfinity);

            //Calculate nearest point on spline  nearest to player (or other gravity affected object)
            using var native = new NativeSpline(
                splineToUse.Spline,
                splineToUse.transform.localToWorldMatrix
            );
            var d = SplineUtility.GetNearestPoint(
                native,
                other.transform.position,
                out var p,
                out var t
            );
            if (d < nearest.w)
                nearest = new float4(p, d);

            nearestPointOnSpline.position = nearest.xyz;

            //Calculate vector from player to the closest point and send it over
            var pointAtNearest =
                -1
                * gravIntensity
                * (other.transform.position - nearestPointOnSpline.position).normalized;
            var gravPackageToSend = new GravPackageStruct(pointAtNearest, priority);
            gravityReceiver.AddGravStruct(gravPackageToSend);
        }
    }
}
