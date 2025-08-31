using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class Tracer
    {
        const float NormalRayDistance = 0.002f;
        const float NormalRayOffset = 0.001f;
        const float CapsuleVerticalOffset = 0.5f;

        public static Trace TraceCollider(
            Collider collider,
            Vector3 origin,
            Vector3 end,
            int layerMask,
            float colliderScale = 1f
        )
        {
            return collider switch
            {
                BoxCollider box
                    => TraceBox(
                        origin,
                        end,
                        box.bounds.extents,
                        box.contactOffset,
                        layerMask,
                        colliderScale
                    ),
                CapsuleCollider capsule
                    => TraceCapsuleCollider(capsule, origin, end, layerMask, colliderScale),
                _
                    => throw new NotImplementedException(
                        $"Trace missing for collider: {collider.GetType()}"
                    )
            };
        }

        static Trace TraceCapsuleCollider(
            CapsuleCollider capsule,
            Vector3 origin,
            Vector3 end,
            int layerMask,
            float colliderScale
        )
        {
            SurfPhysics.GetCapsulePoints(capsule, origin, out var point1, out var point2);
            return TraceCapsule(
                point1,
                point2,
                capsule.radius,
                origin,
                end,
                capsule.contactOffset,
                layerMask,
                colliderScale
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
            var result = CreateTrace(start, destination);
            var direction = CalculateDirection(start, destination);
            var maxDistance = CalculateMaxDistance(start, destination, contactOffset);
            radius *= 1f - contactOffset;
            var capsulePoint1 = point1 - (CapsuleVerticalOffset * colliderScale * Vector3.up);
            var capsulePoint2 = point2 + (CapsuleVerticalOffset * colliderScale * Vector3.up);
            var scaledRadius = radius * colliderScale;
            if (
                PerformCapsuleCast(
                    capsulePoint1,
                    capsulePoint2,
                    scaledRadius,
                    direction,
                    maxDistance,
                    layerMask,
                    out var hit
                )
            )
                PopulateTraceFromHit(ref result, hit, maxDistance, direction);
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
            var result = CreateTrace(start, destination);
            var direction = CalculateDirection(start, destination);
            var maxDistance = CalculateMaxDistance(start, destination, contactOffset);
            var scaledExtents = (1f - contactOffset) * colliderScale * extents;
            if (
                PerformBoxCast(start, scaledExtents, direction, maxDistance, layerMask, out var hit)
            )
                PopulateTraceFromHit(ref result, hit, maxDistance, direction);
            else
                result.fraction = 1f;
            return result;
        }

        static Trace CreateTrace(Vector3 start, Vector3 destination) =>
            new()
            {
                startPos = start,
                endPos = destination,
                fraction = 0f,
                hitCollider = null,
                hitPoint = Vector3.zero,
                planeNormal = Vector3.zero,
                distance = 0f
            };

        static Vector3 CalculateDirection(Vector3 start, Vector3 destination) =>
            (destination - start).normalized;

        static float CalculateMaxDistance(Vector3 start, Vector3 destination, float contactOffset)
        {
            var baseDistance = Vector3.Distance(start, destination);
            var longSide = Mathf.Sqrt(contactOffset * contactOffset * 2f);
            return baseDistance + longSide;
        }

        static bool PerformCapsuleCast(
            Vector3 point1,
            Vector3 point2,
            float radius,
            Vector3 direction,
            float maxDistance,
            int layerMask,
            out RaycastHit hit
        ) =>
            Physics.CapsuleCast(
                point1,
                point2,
                radius,
                direction,
                out hit,
                maxDistance,
                layerMask,
                QueryTriggerInteraction.Ignore
            );

        static bool PerformBoxCast(
            Vector3 center,
            Vector3 extents,
            Vector3 direction,
            float maxDistance,
            int layerMask,
            out RaycastHit hit
        ) =>
            Physics.BoxCast(
                center,
                extents,
                direction,
                out hit,
                Quaternion.identity,
                maxDistance,
                layerMask,
                QueryTriggerInteraction.Ignore
            );

        static void PopulateTraceFromHit(
            ref Trace trace,
            RaycastHit hit,
            float maxDistance,
            Vector3 direction
        )
        {
            trace.fraction = hit.distance / maxDistance;
            trace.hitCollider = hit.collider;
            trace.hitPoint = hit.point;
            trace.planeNormal = hit.normal;
            trace.distance = hit.distance;
            RefineNormalUsingRaycast(hit, direction, ref trace);
        }

        static void RefineNormalUsingRaycast(RaycastHit hit, Vector3 direction, ref Trace trace)
        {
            var normalRay = new Ray(hit.point - (direction * NormalRayOffset), direction);
            if (hit.collider.Raycast(normalRay, out var normalHit, NormalRayDistance))
                trace.planeNormal = normalHit.normal;
        }
    }
}
