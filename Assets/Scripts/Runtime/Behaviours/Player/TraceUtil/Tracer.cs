using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class Tracer
    {
        public static Trace TraceCollider(
            Collider collider,
            Vector3 origin,
            Vector3 end,
            int layerMask,
            float colliderScale = 1f
        )
        {
            if (collider is BoxCollider)
                // Box collider trace
                return TraceBox(
                    origin,
                    end,
                    collider.bounds.extents,
                    collider.contactOffset,
                    layerMask,
                    colliderScale
                );
            else if (collider is CapsuleCollider capc) // Capsule collider trace
            {
                SurfPhysics.GetCapsulePoints(capc, origin, out var point1, out var point2);
                return TraceCapsule(
                    point1,
                    point2,
                    capc.radius,
                    origin,
                    end,
                    capc.contactOffset,
                    layerMask,
                    colliderScale
                );
            }

            throw new System.NotImplementedException(
                "Trace missing for collider: " + collider.GetType()
            );
        }

        public static Trace TraceCapsule(
            Vector3 point1,
            Vector3 point2,
            float radius,
            Vector3 start,
            Vector3 destination,
            float contactOffset,
            int layerMask,
            float colliderScale = 1f
        )
        {
            var result = new Trace() { startPos = start, endPos = destination };
            var longSide = Mathf.Sqrt(
                contactOffset * contactOffset + contactOffset * contactOffset
            );
            radius *= 1f - contactOffset;
            var direction = (destination - start).normalized;
            var maxDistance = Vector3.Distance(start, destination) + longSide;

            if (
                Physics.CapsuleCast(
                    point1 - 0.5f * colliderScale * Vector3.up,
                    point2 + 0.5f * colliderScale * Vector3.up,
                    radius * colliderScale,
                    direction,
                    out var hit,
                    maxDistance,
                    layerMask,
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                result.fraction = hit.distance / maxDistance;
                result.hitCollider = hit.collider;
                result.hitPoint = hit.point;
                result.planeNormal = hit.normal;
                result.distance = hit.distance;

                var normalRay = new Ray(hit.point - direction * 0.001f, direction);
                if (hit.collider.Raycast(normalRay, out var normalHit, 0.002f))
                    result.planeNormal = normalHit.normal;
            }
            else
                result.fraction = 1f;

            return result;
        }

        public static Trace TraceBox(
            Vector3 start,
            Vector3 destination,
            Vector3 extents,
            float contactOffset,
            int layerMask,
            float colliderScale = 1f
        )
        {
            var result = new Trace() { startPos = start, endPos = destination };
            var longSide = Mathf.Sqrt(
                contactOffset * contactOffset + contactOffset * contactOffset
            );
            var direction = (destination - start).normalized;
            var maxDistance = Vector3.Distance(start, destination) + longSide;
            extents *= 1f - contactOffset;

            if (
                Physics.BoxCast(
                    start,
                    extents * colliderScale,
                    direction,
                    out var hit,
                    Quaternion.identity,
                    maxDistance,
                    layerMask,
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                result.fraction = hit.distance / maxDistance;
                result.hitCollider = hit.collider;
                result.hitPoint = hit.point;
                result.planeNormal = hit.normal;
                result.distance = hit.distance;

                var normalRay = new Ray(hit.point - direction * 0.001f, direction);
                if (hit.collider.Raycast(normalRay, out var normalHit, 0.002f))
                    result.planeNormal = normalHit.normal;
            }
            else
                result.fraction = 1f;

            return result;
        }
    }
}
