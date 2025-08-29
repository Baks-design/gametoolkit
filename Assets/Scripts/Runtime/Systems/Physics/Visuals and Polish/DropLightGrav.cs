using UnityEngine;
using UnityEngine.Rendering.Universal;
using UPhysics = UnityEngine.Physics;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    /// Script for drawing a drop shadow based on gravity instead of the scene lighting.
    /// Note that the URP decals aren't especially efficient - if you run into performance issues,
    /// it might be because of this script, which is why I have the dynamic scaling/opacity as optional.";
    /// </summary>
    [RequireComponent(typeof(CustomGravityAffected))]
    public class DropLightGrav : MonoBehaviour
    {
        [Header("Important stuff for drawing the shadow")]
        [SerializeField]
        CustomGravityAffected gravScript;

        [SerializeField]
        GameObject gravLight;

        [SerializeField]
        LayerMask layersToDrawOn;

        [Header("Scaling/Opacity on Shadow")]
        [SerializeField]
        bool dynamicallyScaleOpacitySize = false;

        [SerializeField]
        float longestDistance;

        [SerializeField]
        float shortestDistance;

        [SerializeField]
        float lowestOpacity = 0.2f;

        [SerializeField]
        float highestOpacity = 0.9f;

        [SerializeField]
        float biggestSize = 4.5f;

        [SerializeField]
        float smallestSize = 2f;

        [SerializeField]
        float shadowDepth = 1.39f;

        [SerializeField]
        AnimationCurve tweenCurve;

        [SerializeField]
        DecalProjector shadowProjector;

        void Start()
        {
            if (gravScript == null)
                TryGetComponent(out gravScript);

            if (shadowProjector == null)
                gravLight.TryGetComponent(out shadowProjector);
        }

        void Update()
        {
            //Draw a raycast down utilizing the gravity script,
            // then move the shadow object to the first point that the raycast hits
            if (
                UPhysics.Raycast(
                    transform.position,
                    gravScript.FallingDirection,
                    out var objHit,
                    Mathf.Infinity,
                    layersToDrawOn
                )
            )
            {
                gravLight.transform.position = objHit.point;
                //Set size and opacity of shadow based on height
                if (dynamicallyScaleOpacitySize)
                {
                    var usableRange = Mathf.Clamp(
                        objHit.distance,
                        shortestDistance,
                        longestDistance
                    );
                    var curveValue = tweenCurve.Evaluate(usableRange / longestDistance);
                    shadowProjector.fadeFactor = Mathf.Lerp(
                        highestOpacity,
                        lowestOpacity,
                        curveValue
                    );
                    var sizeVal = Mathf.Lerp(smallestSize, biggestSize, curveValue);
                    shadowProjector.size = new Vector3(sizeVal, sizeVal, shadowDepth);
                }
            }
        }
    }
}
