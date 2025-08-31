using GameToolkit.Runtime.Utils.Extensions;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class SurfPhysics
    {
#pragma warning disable UDR0001 // Domain Reload Analyzer
        public static int groundLayerMask = LayerMask.GetMask("Default", "Ground", "Player");
        const int maxCollisions = 128;
        const int maxClipPlanes = 5;
        const float SurfSlope = 0.7f;
        const int numBumps = 1;
        static readonly Collider[] colliders = new Collider[maxCollisions];
        static readonly Vector3[] planes = new Vector3[maxClipPlanes];
#pragma warning restore UDR0001 // Domain Reload Analyzer

        public static void ResolveCollisions(
            Collider collider,
            ref Vector3 origin,
            ref Vector3 velocity,
            float rigidbodyPushForce,
            float velocityMultiplier = 1f,
            float stepOffset = 0f,
            ISurfControllable surfer = null
        )
        {
            var numOverlaps = GetOverlaps(collider, origin);
            var forwardVelocity = Vector3.Scale(velocity, new Vector3(1f, 0f, 1f));

            for (var i = 0; i < numOverlaps; i++)
            {
                if (
                    TryHandleCollision(
                        collider,
                        ref origin,
                        ref velocity,
                        colliders[i],
                        forwardVelocity,
                        stepOffset,
                        surfer,
                        rigidbodyPushForce,
                        velocityMultiplier
                    )
                )
                    return;
            }
        }

        static int GetOverlaps(Collider collider, Vector3 origin)
        {
            switch (collider)
            {
                case CapsuleCollider capsule:
                    GetCapsulePoints(capsule, origin, out var point1, out var point2);
                    return Physics.OverlapCapsuleNonAlloc(
                        point1,
                        point2,
                        capsule.radius,
                        colliders,
                        groundLayerMask,
                        QueryTriggerInteraction.Ignore
                    );

                case BoxCollider:
                    return Physics.OverlapBoxNonAlloc(
                        origin,
                        collider.bounds.extents,
                        colliders,
                        Quaternion.identity,
                        groundLayerMask,
                        QueryTriggerInteraction.Ignore
                    );

                default:
                    return 0;
            }
        }

        static bool TryHandleCollision(
            Collider collider,
            ref Vector3 origin,
            ref Vector3 velocity,
            Collider otherCollider,
            Vector3 forwardVelocity,
            float stepOffset,
            ISurfControllable surfer,
            float rigidbodyPushForce,
            float velocityMultiplier
        )
        {
            if (
                !Physics.ComputePenetration(
                    collider,
                    origin,
                    Quaternion.identity,
                    otherCollider,
                    otherCollider.transform.position,
                    otherCollider.transform.rotation,
                    out var direction,
                    out var distance
                )
            )
                return false;

            // Step offset
            if (stepOffset > 0f && surfer != null && surfer.MoveData.useStepOffset)
                if (
                    StepOffset(
                        collider,
                        ref origin,
                        ref velocity,
                        stepOffset,
                        forwardVelocity,
                        surfer
                    )
                )
                    return true;

            // Handle collision
            direction.Normalize();
            var penetrationVector = direction * distance;
            var velocityProjected = Vector3.Project(velocity, -direction);
            velocityProjected.y = 0f; // don't touch y velocity

            origin += penetrationVector;
            velocity -= velocityProjected * velocityMultiplier;

            HandleRigidbodyPush(
                otherCollider,
                origin,
                velocityProjected,
                rigidbodyPushForce,
                velocityMultiplier
            );

            return false;
        }

        static void HandleRigidbodyPush(
            Collider otherCollider,
            Vector3 origin,
            Vector3 velocityProjected,
            float rigidbodyPushForce,
            float velocityMultiplier
        )
        {
            var rb = otherCollider.GetComponentInParent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
                rb.AddForceAtPosition(
                    rigidbodyPushForce * velocityMultiplier * velocityProjected,
                    origin,
                    ForceMode.Impulse
                );
        }

        public static bool StepOffset(
            Collider collider,
            ref Vector3 origin,
            ref Vector3 velocity,
            float stepOffset,
            Vector3 forwardVelocity,
            ISurfControllable surfer
        )
        {
            if (stepOffset <= 0f)
                return false;

            var forwardDirection = forwardVelocity.normalized;
            if (forwardDirection.sqrMagnitude == 0f)
                return false;

            // Trace ground
            var groundTrace = Tracer.TraceCollider(
                collider,
                origin,
                origin + Vector3.down * 0.1f,
                groundLayerMask
            );
            if (
                groundTrace.hitCollider == null
                || Vector3.Angle(Vector3.up, groundTrace.planeNormal) > surfer.MoveData.slopeLimit
            )
                return false;

            // Trace wall
            var wallTrace = Tracer.TraceCollider(
                collider,
                origin,
                origin + velocity,
                groundLayerMask,
                0.9f
            );
            if (
                wallTrace.hitCollider == null
                || Vector3.Angle(Vector3.up, wallTrace.planeNormal) <= surfer.MoveData.slopeLimit
            )
                return false;

            // Trace upwards
            var upDistance = stepOffset;
            var upTrace = Tracer.TraceCollider(
                collider,
                origin,
                origin + Vector3.up * stepOffset,
                groundLayerMask
            );
            if (upTrace.hitCollider != null)
                upDistance = upTrace.distance;
            if (upDistance <= 0f)
                return false;

            // Trace forwards
            var upOrigin = origin + Vector3.up * upDistance;
            var forwardMagnitude = stepOffset;
            var forwardDistance = forwardMagnitude;
            var forwardTrace = Tracer.TraceCollider(
                collider,
                upOrigin,
                upOrigin + forwardDirection * Mathf.Max(0.2f, forwardMagnitude),
                groundLayerMask
            );
            if (forwardTrace.hitCollider != null)
                forwardDistance = forwardTrace.distance;
            if (forwardDistance <= 0f)
                return false;

            // Trace down
            var upForwardOrigin = upOrigin + forwardDirection * forwardDistance;
            var downDistance = upDistance;
            var downTrace = Tracer.TraceCollider(
                collider,
                upForwardOrigin,
                upForwardOrigin + Vector3.down * upDistance,
                groundLayerMask
            );
            if (downTrace.hitCollider != null)
                downDistance = downTrace.distance;

            // Check step size/angle
            var verticalStep = Mathf.Clamp(upDistance - downDistance, 0f, stepOffset);
            var horizontalStep = forwardDistance;
            var dirStep = Vector3.zero;
            dirStep.Set(0f, verticalStep, horizontalStep);
            var stepAngle = Vector3.Angle(Vector3.forward, dirStep);
            if (stepAngle > surfer.MoveData.slopeLimit)
                return false;

            // Get new position and move
            var endOrigin = origin + Vector3.up * verticalStep;
            if (origin != endOrigin && forwardDistance > 0f)
            {
                Logging.Log("Moved up step!");
                origin = endOrigin + forwardDistance * Time.deltaTime * forwardDirection;
                return true;
            }

            return false;
        }

        public static void Friction(
            ref Vector3 velocity,
            float stopSpeed,
            float friction,
            float deltaTime
        )
        {
            var speed = velocity.magnitude;
            if (speed < 0.0001905f)
                return;
            var control = speed < stopSpeed ? stopSpeed : speed;
            var drop = control * friction * deltaTime;
            var newspeed = Mathf.Max(speed - drop, 0f);
            if (newspeed != speed)
                velocity *= newspeed / speed;
        }

        public static Vector3 AirAccelerate(
            Vector3 velocity,
            Vector3 wishdir,
            float wishspeed,
            float accel,
            float airCap,
            float deltaTime
        )
        {
            var wishspd = Mathf.Min(wishspeed, airCap);
            var currentspeed = Vector3.Dot(velocity, wishdir);
            var addspeed = wishspd - currentspeed;
            if (addspeed <= 0f)
                return Vector3.zero;
            var accelspeed = Mathf.Min(accel * wishspeed * deltaTime, addspeed);
            return wishdir * accelspeed;
        }

        public static Vector3 Accelerate(
            Vector3 currentVelocity,
            Vector3 wishdir,
            float wishspeed,
            float accel,
            float deltaTime,
            float surfaceFriction
        )
        {
            var currentspeed = Vector3.Dot(currentVelocity, wishdir);
            var addspeed = wishspeed - currentspeed;
            if (addspeed <= 0f)
                return Vector3.zero;
            var accelspeed = Mathf.Min(accel * deltaTime * wishspeed * surfaceFriction, addspeed);
            return wishdir * accelspeed;
        }

        public static int Reflect(
            ref Vector3 velocity,
            Collider collider,
            Vector3 origin,
            float deltaTime
        )
        {
            var blocked = 0;
            var numplanes = 0;
            var originalVelocity = velocity;
            var primalVelocity = velocity;
            var allFraction = 0f;
            var timeLeft = deltaTime;

            for (var bumpcount = 0; bumpcount < numBumps; bumpcount++)
            {
                if (velocity.magnitude == 0f)
                    break;

                var end = VectorExtensions.VectorMa(origin, timeLeft, velocity);
                var trace = Tracer.TraceCollider(collider, origin, end, groundLayerMask);

                allFraction += trace.fraction;

                if (trace.fraction > 0f)
                {
                    originalVelocity = velocity;
                    numplanes = 0;
                }

                if (trace.fraction == 1f)
                    break;

                // Update blocked flags
                if (trace.planeNormal.y > SurfSlope)
                    blocked |= 1;
                if (trace.planeNormal.y == 0f)
                    blocked |= 2;

                timeLeft -= timeLeft * trace.fraction;

                if (numplanes >= maxClipPlanes)
                {
                    velocity = Vector3.zero;
                    break;
                }

                planes[numplanes++] = trace.planeNormal;

                if (numplanes == 1)
                    HandleSinglePlaneReflection(ref velocity, originalVelocity, numplanes);
                else
                {
                    HandleMultiPlaneReflection(
                        ref velocity,
                        originalVelocity,
                        primalVelocity,
                        numplanes,
                        ref blocked
                    );
                }
            }

            if (allFraction == 0f)
                velocity = Vector3.zero;

            return blocked;
        }

        static void HandleSinglePlaneReflection(
            ref Vector3 velocity,
            Vector3 originalVelocity,
            int numplanes
        )
        {
            var newVelocity = Vector3.zero;
            for (var i = 0; i < numplanes; i++)
            {
                if (planes[i].y > SurfSlope)
                    return;
                ClipVelocity(originalVelocity, planes[i], ref newVelocity, 1f);
            }
            velocity = newVelocity;
        }

        static void HandleMultiPlaneReflection(
            ref Vector3 velocity,
            Vector3 originalVelocity,
            Vector3 primalVelocity,
            int numplanes,
            ref int blocked
        )
        {
            var foundValidClip = false;

            for (var i = 0; i < numplanes; i++)
            {
                ClipVelocity(originalVelocity, planes[i], ref velocity, 1f);

                var validClip = true;
                for (var j = 0; j < numplanes; j++)
                {
                    if (j != i && Vector3.Dot(velocity, planes[j]) < 0f)
                    {
                        validClip = false;
                        break;
                    }
                }

                if (validClip)
                {
                    foundValidClip = true;
                    break;
                }
            }

            if (!foundValidClip)
            {
                if (numplanes == 2)
                {
                    var dir = Vector3.Cross(planes[0], planes[1]).normalized;
                    var d = Vector3.Dot(dir, velocity);
                    velocity = dir * d;
                }
                else
                {
                    velocity = Vector3.zero;
                    blocked |= 4; // Add flag for complex clipping scenario
                }
            }

            if (Vector3.Dot(velocity, primalVelocity) <= 0f)
                velocity = Vector3.zero;
        }

        public static int ClipVelocity(
            Vector3 input,
            Vector3 normal,
            ref Vector3 output,
            float overbounce
        )
        {
            var blocked = 0;
            if (normal.y > 0f)
                blocked |= 0x01;
            if (normal.y == 0f)
                blocked |= 0x02;

            var backoff = Vector3.Dot(input, normal) * overbounce;
            output = input - (normal * backoff);

            // Ensure we aren't still moving through the plane
            var adjust = Vector3.Dot(output, normal);
            if (adjust < 0f)
                output -= normal * adjust;

            return blocked;
        }

        public static void GetCapsulePoints(
            CapsuleCollider capc,
            Vector3 origin,
            out Vector3 p1,
            out Vector3 p2
        )
        {
            var distanceToPoints = capc.height * 0.5f - capc.radius;
            p1 = origin + capc.center + Vector3.up * distanceToPoints;
            p2 = origin + capc.center - Vector3.up * distanceToPoints;
        }
    }
}
