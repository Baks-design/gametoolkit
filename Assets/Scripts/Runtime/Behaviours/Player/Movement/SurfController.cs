using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class SurfController
    {
        [HideInInspector]
        public Transform playerTransform;
        public Transform camera;
        public float speed = 0f;
        public float cameraYPos = 0f;
        ISurfControllable surfer;
        MovementConfig config;
        Vector3 slideDirection = Vector3.forward;
        Vector3 groundNormal = Vector3.up;
        readonly float frictionMult = 1f;
        float deltaTime;
        float slideSpeedCurrent = 0f;
        float slideDelay = 0f;
        float crouchLerp = 0f;

        public bool Sliding { get; private set; } = false;
        public bool WasSliding { get; private set; } = false;
        public bool UncrouchDown { get; private set; } = false;
        public bool Jumping { get; private set; } = false;
        public bool Crouching { get; private set; } = false;

        public void ProcessMovement(
            ISurfControllable surfer,
            MovementConfig config,
            float deltaTime
        )
        {
            // cache instead of passing around parameters
            this.surfer = surfer;
            this.config = config;
            this.deltaTime = deltaTime;

            if (surfer.MoveData.laddersEnabled && !surfer.MoveData.climbingLadder)
                // Look for ladders
                LadderCheck(
                    new Vector3(1f, 0.95f, 1f),
                    surfer.MoveData.velocity * Mathf.Clamp(Time.deltaTime * 2f, 0.025f, 0.25f)
                );

            if (surfer.MoveData.laddersEnabled && surfer.MoveData.climbingLadder)
                LadderPhysics();
            else if (!surfer.MoveData.underwater)
            {
                if (surfer.MoveData.velocity.y <= 0f)
                    Jumping = false;

                // apply gravity
                if (surfer.GroundObject == null)
                {
                    surfer.MoveData.velocity.y -=
                        surfer.MoveData.gravityFactor * config.gravity * deltaTime;
                    surfer.MoveData.velocity.y += surfer.BaseVelocity.y * deltaTime;
                }

                // input velocity, check for ground
                CheckGrounded();
                CalculateMovementVelocity();
            }
            else
                // Do underwater logic
                UnderwaterPhysics();

            var yVel = surfer.MoveData.velocity.y;
            surfer.MoveData.velocity.y = 0f;
            surfer.MoveData.velocity = Vector3.ClampMagnitude(
                surfer.MoveData.velocity,
                config.maxVelocity
            );
            speed = surfer.MoveData.velocity.magnitude;
            surfer.MoveData.velocity.y = yVel;

            if (surfer.MoveData.velocity.sqrMagnitude == 0f)
                // Do collisions while standing still
                SurfPhysics.ResolveCollisions(
                    surfer.Collider,
                    ref surfer.MoveData.origin,
                    ref surfer.MoveData.velocity,
                    surfer.MoveData.rigidbodyPushForce,
                    1f,
                    surfer.MoveData.stepOffset,
                    surfer
                );
            else
            {
                var maxDistPerFrame = 0.2f;
                var velocityThisFrame = surfer.MoveData.velocity * deltaTime;
                var velocityDistLeft = velocityThisFrame.magnitude;
                var initialVel = velocityDistLeft;

                while (velocityDistLeft > 0f)
                {
                    var amountThisLoop = Mathf.Min(maxDistPerFrame, velocityDistLeft);
                    velocityDistLeft -= amountThisLoop;

                    // increment origin
                    var velThisLoop = velocityThisFrame * (amountThisLoop / initialVel);
                    surfer.MoveData.origin += velThisLoop;

                    // don't penetrate walls
                    SurfPhysics.ResolveCollisions(
                        surfer.Collider,
                        ref surfer.MoveData.origin,
                        ref surfer.MoveData.velocity,
                        surfer.MoveData.rigidbodyPushForce,
                        amountThisLoop / initialVel,
                        surfer.MoveData.stepOffset,
                        surfer
                    );
                }
            }

            surfer.MoveData.groundedTemp = surfer.MoveData.grounded;
        }

        void CalculateMovementVelocity()
        {
            switch (surfer.MoveType)
            {
                case MoveType.Walk:

                    if (surfer.GroundObject == null) // AIR MOVEMENT
                    {
                        WasSliding = false;

                        // apply movement from input
                        surfer.MoveData.velocity += AirInputMovement();

                        // let the magic happen
                        SurfPhysics.Reflect(
                            ref surfer.MoveData.velocity,
                            surfer.Collider,
                            surfer.MoveData.origin,
                            deltaTime
                        );
                    }
                    else // GROUND MOVEMENT
                    {
                        // Sliding
                        if (!WasSliding)
                        {
                            slideDirection = new Vector3(
                                surfer.MoveData.velocity.x,
                                0f,
                                surfer.MoveData.velocity.z
                            ).normalized;

                            slideSpeedCurrent = Mathf.Max(
                                config.maximumSlideSpeed,
                                new Vector3(
                                    surfer.MoveData.velocity.x,
                                    0f,
                                    surfer.MoveData.velocity.z
                                ).magnitude
                            );
                        }

                        Sliding = false;
                        if (
                            surfer.MoveData.velocity.magnitude > config.minimumSlideSpeed
                            && surfer.MoveData.slidingEnabled
                            && surfer.MoveData.crouching
                            && slideDelay <= 0f
                        )
                        {
                            if (!WasSliding)
                                slideSpeedCurrent = Mathf.Clamp(
                                    slideSpeedCurrent * config.slideSpeedMultiplier,
                                    config.minimumSlideSpeed,
                                    config.maximumSlideSpeed
                                );

                            Sliding = true;
                            WasSliding = true;
                            SlideMovement();

                            return;
                        }
                        else
                        {
                            if (slideDelay > 0f)
                                slideDelay -= deltaTime;

                            if (WasSliding)
                                slideDelay = config.slideDelay;

                            WasSliding = false;
                        }

                        var fric = Crouching ? config.crouchFriction : config.friction;
                        var accel = Crouching ? config.crouchAcceleration : config.acceleration;
                        var decel = Crouching ? config.crouchDeceleration : config.deceleration;

                        // Get movement directions
                        var forward = Vector3.Cross(groundNormal, -playerTransform.right);
                        var right = Vector3.Cross(groundNormal, forward);

                        var speed = surfer.MoveData.sprinting
                            ? config.sprintSpeed
                            : config.walkSpeed;
                        if (Crouching)
                            speed = config.crouchSpeed;

                        // Jump and friction
                        if (surfer.MoveData.wishJump)
                        {
                            ApplyFriction(0f, true, true);
                            Jump();
                            return;
                        }
                        else
                            ApplyFriction(1f * frictionMult, true, true);

                        var forwardMove = surfer.MoveData.verticalAxis;
                        var rightMove = surfer.MoveData.horizontalAxis;

                        var wishDir = forwardMove * forward + rightMove * right;
                        wishDir.Normalize();

                        var forwardVelocity = Vector3.Cross(
                            groundNormal,
                            Quaternion.AngleAxis(-90f, Vector3.up)
                                * new Vector3(
                                    surfer.MoveData.velocity.x,
                                    0f,
                                    surfer.MoveData.velocity.z
                                )
                        );

                        // Set the target speed of the player
                        var wishSpeed = wishDir.magnitude;
                        wishSpeed *= speed;

                        // Accelerate
                        var yVel = surfer.MoveData.velocity.y;
                        Accelerate(wishDir, wishSpeed, accel * Mathf.Min(frictionMult, 1f), false);

                        var maxVelocityMagnitude = config.maxVelocity;
                        surfer.MoveData.velocity = Vector3.ClampMagnitude(
                            new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z),
                            maxVelocityMagnitude
                        );
                        surfer.MoveData.velocity.y = yVel;

                        // Calculate how much slopes should affect movement
                        var yVelocityNew =
                            forwardVelocity.normalized.y
                            * new Vector3(
                                surfer.MoveData.velocity.x,
                                0f,
                                surfer.MoveData.velocity.z
                            ).magnitude;

                        // Apply the Y-movement from slopes
                        surfer.MoveData.velocity.y = yVelocityNew * (wishDir.y < 0f ? 1.2f : 1f);
                        var removableYVelocity = surfer.MoveData.velocity.y - yVelocityNew;
                    }

                    break;
            }
        }

        void UnderwaterPhysics()
        {
            surfer.MoveData.velocity = Vector3.Lerp(
                surfer.MoveData.velocity,
                Vector3.zero,
                config.underwaterVelocityDampening * deltaTime
            );

            // Gravity
            if (!CheckGrounded())
                surfer.MoveData.velocity.y -= config.underwaterGravity * deltaTime;

            // Swimming upwards
            if (Input.GetButton("Jump"))
                surfer.MoveData.velocity.y += config.swimUpSpeed * deltaTime;

            float fric = config.underwaterFriction;
            float accel = config.underwaterAcceleration;
            float decel = config.underwaterDeceleration;

            ApplyFriction(1f, true, false);

            // Get movement directions
            var forward = Vector3.Cross(groundNormal, -playerTransform.right);
            var right = Vector3.Cross(groundNormal, forward);

            var speed = config.underwaterSwimSpeed;

            var forwardMove = surfer.MoveData.verticalAxis;
            var rightMove = surfer.MoveData.horizontalAxis;

            Vector3 wishDir;
            wishDir = forwardMove * forward + rightMove * right;
            wishDir.Normalize();

            var forwardVelocity = Vector3.Cross(
                groundNormal,
                Quaternion.AngleAxis(-90f, Vector3.up)
                    * new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z)
            );

            // Set the target speed of the player
            var wishSpeed = wishDir.magnitude;
            wishSpeed *= speed;

            // Accelerate
            var yVel = surfer.MoveData.velocity.y;
            Accelerate(wishDir, wishSpeed, accel, false);

            var maxVelocityMagnitude = config.maxVelocity;
            surfer.MoveData.velocity = Vector3.ClampMagnitude(
                new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z),
                maxVelocityMagnitude
            );
            surfer.MoveData.velocity.y = yVel;

            var yVelStored = surfer.MoveData.velocity.y;
            surfer.MoveData.velocity.y = 0f;

            // Calculate how much slopes should affect movement
            var yVelocityNew =
                forwardVelocity.normalized.y
                * new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z).magnitude;

            // Apply the Y-movement from slopes
            surfer.MoveData.velocity.y = Mathf.Min(Mathf.Max(0f, yVelocityNew) + yVelStored, speed);

            // Jumping out of water
            var movingForwards =
                playerTransform.InverseTransformVector(surfer.MoveData.velocity).z > 0f;
            var waterJumpTrace = TraceBounds(
                playerTransform.position,
                playerTransform.position + playerTransform.forward * 0.1f,
                SurfPhysics.groundLayerMask
            );
            if (
                waterJumpTrace.hitCollider != null
                && Vector3.Angle(Vector3.up, waterJumpTrace.planeNormal) >= config.slopeLimit
                && Input.GetButton("Jump")
                && !surfer.MoveData.cameraUnderwater
                && movingForwards
            )
                surfer.MoveData.velocity.y = Mathf.Max(
                    surfer.MoveData.velocity.y,
                    config.jumpForce
                );
        }

        void LadderCheck(Vector3 colliderScale, Vector3 direction)
        {
            if (surfer.MoveData.velocity.sqrMagnitude <= 0f)
                return;

            var foundLadder = false;

            var rays = new RaycastHit[12];
            var hits = Physics.BoxCastNonAlloc(
                surfer.MoveData.origin,
                Vector3.Scale(surfer.Collider.bounds.size * 0.5f, colliderScale),
                Vector3.Scale(direction, new Vector3(1f, 0f, 1f)),
                rays,
                Quaternion.identity,
                direction.magnitude,
                SurfPhysics.groundLayerMask,
                QueryTriggerInteraction.Collide
            );
            for (var i = 0; i < hits; i++)
            {
                var ladder = rays[i].transform.GetComponentInParent<Ladder>();
                if (ladder != null)
                {
                    var allowClimb = true;
                    var ladderAngle = Vector3.Angle(Vector3.up, rays[i].normal);
                    if (surfer.MoveData.angledLaddersEnabled)
                    {
                        if (rays[i].normal.y < 0f)
                            allowClimb = false;
                        else if (ladderAngle <= surfer.MoveData.slopeLimit)
                            allowClimb = false;
                    }
                    else if (rays[i].normal.y != 0f)
                        allowClimb = false;

                    if (allowClimb)
                    {
                        foundLadder = true;
                        if (surfer.MoveData.climbingLadder == false)
                        {
                            surfer.MoveData.climbingLadder = true;
                            surfer.MoveData.ladderNormal = rays[i].normal;
                            surfer.MoveData.ladderDirection =
                                2f * direction.magnitude * -rays[i].normal;

                            if (surfer.MoveData.angledLaddersEnabled)
                            {
                                var sideDir = rays[i].normal;
                                sideDir.y = 0f;
                                sideDir = Quaternion.AngleAxis(-90f, Vector3.up) * sideDir;

                                surfer.MoveData.ladderClimbDir =
                                    Quaternion.AngleAxis(90f, sideDir) * rays[i].normal;
                                surfer.MoveData.ladderClimbDir *=
                                    1f / surfer.MoveData.ladderClimbDir.y; // Make sure Y is always 1
                            }
                            else
                                surfer.MoveData.ladderClimbDir = Vector3.up;
                        }
                    }
                }
            }

            if (!foundLadder)
            {
                surfer.MoveData.ladderNormal = Vector3.zero;
                surfer.MoveData.ladderVelocity = Vector3.zero;
                surfer.MoveData.climbingLadder = false;
                surfer.MoveData.ladderClimbDir = Vector3.up;
            }
        }

        void LadderPhysics()
        {
            surfer.MoveData.ladderVelocity =
                6f * surfer.MoveData.verticalAxis * surfer.MoveData.ladderClimbDir;

            surfer.MoveData.velocity = Vector3.Lerp(
                surfer.MoveData.velocity,
                surfer.MoveData.ladderVelocity,
                Time.deltaTime * 10f
            );

            LadderCheck(Vector3.one, surfer.MoveData.ladderDirection);

            var floorTrace = TraceToFloor();
            if (
                surfer.MoveData.verticalAxis < 0f
                && floorTrace.hitCollider != null
                && Vector3.Angle(Vector3.up, floorTrace.planeNormal) <= surfer.MoveData.slopeLimit
            )
            {
                surfer.MoveData.velocity = surfer.MoveData.ladderNormal * 0.5f;
                surfer.MoveData.ladderVelocity = Vector3.zero;
                surfer.MoveData.climbingLadder = false;
            }

            if (surfer.MoveData.wishJump)
            {
                surfer.MoveData.velocity = surfer.MoveData.ladderNormal * 4f;
                surfer.MoveData.ladderVelocity = Vector3.zero;
                surfer.MoveData.climbingLadder = false;
            }
        }

        void Accelerate(Vector3 wishDir, float wishSpeed, float acceleration, bool yMovement)
        {
            // Initialise variables
            float addSpeed;
            float accelerationSpeed;
            float currentSpeed;

            // again, no idea
            currentSpeed = Vector3.Dot(surfer.MoveData.velocity, wishDir);
            addSpeed = wishSpeed - currentSpeed;
            // If you're not actually increasing your speed, stop here.
            if (addSpeed <= 0f)
                return;

            // won't bother trying to understand any of this, really
            accelerationSpeed = Mathf.Min(acceleration * deltaTime * wishSpeed, addSpeed);

            // Add the velocity.
            surfer.MoveData.velocity.x += accelerationSpeed * wishDir.x;
            if (yMovement)
                surfer.MoveData.velocity.y += accelerationSpeed * wishDir.y;
            surfer.MoveData.velocity.z += accelerationSpeed * wishDir.z;
        }

        void ApplyFriction(float t, bool yAffected, bool grounded)
        {
            // Set Y to 0, speed to the magnitude of movement and drop to 0. I think drop is the amount of speed that is lost,
            // but I just stole this from the internet, idk.
            var vel = surfer.MoveData.velocity;
            vel.y = 0f;
            speed = vel.magnitude;
            var drop = 0f;

            var fric = Crouching ? config.crouchFriction : config.friction;
            var accel = Crouching ? config.crouchAcceleration : config.acceleration;
            var decel = Crouching ? config.crouchDeceleration : config.deceleration;

            // Only apply friction if the player is grounded
            if (grounded)
            {
                // i honestly have no idea what this does tbh
                vel.y = surfer.MoveData.velocity.y;
                var control = speed < decel ? decel : speed;
                drop = control * fric * deltaTime * t;
            }

            // again, no idea, but comments look cool
            var newSpeed = Mathf.Max(speed - drop, 0f);
            if (speed > 0f)
                newSpeed /= speed;

            // Set the end-velocity
            surfer.MoveData.velocity.x *= newSpeed;
            if (yAffected == true)
                surfer.MoveData.velocity.y *= newSpeed;
            surfer.MoveData.velocity.z *= newSpeed;
        }

        Vector3 AirInputMovement()
        {
            GetWishValues(out Vector3 wishVel, out Vector3 wishDir, out float wishSpeed);

            if (config.clampAirSpeed && wishSpeed != 0f && (wishSpeed > config.maxSpeed))
            {
                wishVel *= config.maxSpeed / wishSpeed;
                wishSpeed = config.maxSpeed;
            }

            return SurfPhysics.AirAccelerate(
                surfer.MoveData.velocity,
                wishDir,
                wishSpeed,
                config.airAcceleration,
                config.airCap,
                deltaTime
            );
        }

        void GetWishValues(out Vector3 wishVel, out Vector3 wishDir, out float wishSpeed)
        {
            wishVel = Vector3.zero;
            wishDir = Vector3.zero;
            wishSpeed = 0f;

            var forward = surfer.Forward;
            forward[1] = 0f;
            forward.Normalize();
            var right = surfer.Right;
            right[1] = 0f;
            right.Normalize();

            for (var i = 0; i < 3; i++)
                wishVel[i] =
                    forward[i] * surfer.MoveData.forwardMove + right[i] * surfer.MoveData.sideMove;
            wishVel[1] = 0f;

            wishSpeed = wishVel.magnitude;
            wishDir = wishVel.normalized;
        }

        void Jump()
        {
            if (!config.autoBhop)
                surfer.MoveData.wishJump = false;

            surfer.MoveData.velocity.y += config.jumpForce;
            Jumping = true;
        }

        bool CheckGrounded()
        {
            surfer.MoveData.surfaceFriction = 1f;
            var movingUp = surfer.MoveData.velocity.y > 0f;
            var trace = TraceToFloor();
            var groundSteepness = Vector3.Angle(Vector3.up, trace.planeNormal);
            if (
                trace.hitCollider == null
                || groundSteepness > config.slopeLimit
                || (Jumping && surfer.MoveData.velocity.y > 0f)
            )
            {
                SetGround(null);
                if (movingUp && surfer.MoveType != MoveType.Noclip)
                    surfer.MoveData.surfaceFriction = config.airFriction;
                return false;
            }
            else
            {
                groundNormal = trace.planeNormal;
                SetGround(trace.hitCollider.gameObject);
                return true;
            }
        }

        void SetGround(GameObject obj)
        {
            if (obj != null)
            {
                surfer.GroundObject = obj;
                surfer.MoveData.velocity.y = 0f;
            }
            else
                surfer.GroundObject = null;
        }

        Trace TraceBounds(Vector3 start, Vector3 end, int layerMask) =>
            Tracer.TraceCollider(surfer.Collider, start, end, layerMask);

        Trace TraceToFloor()
        {
            var down = surfer.MoveData.origin;
            down.y -= 0.15f;
            return Tracer.TraceCollider(
                surfer.Collider,
                surfer.MoveData.origin,
                down,
                SurfPhysics.groundLayerMask
            );
        }

        public void Crouch(ISurfControllable surfer, MovementConfig config, float deltaTime)
        {
            this.surfer = surfer;
            this.config = config;
            this.deltaTime = deltaTime;

            if (surfer == null)
                return;
            if (surfer.Collider == null)
                return;

            var grounded = surfer.GroundObject != null;
            var wantsToCrouch = surfer.MoveData.crouching;

            var crouchingHeight = Mathf.Clamp(surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var heightDifference =
                surfer.MoveData.defaultHeight - surfer.MoveData.defaultHeight * crouchingHeight;

            // Crouching input
            if (grounded)
                UncrouchDown = false;
            if (grounded)
                crouchLerp = Mathf.Lerp(
                    crouchLerp,
                    wantsToCrouch ? 1f : 0f,
                    deltaTime * surfer.MoveData.crouchingSpeed
                );
            else if (!grounded && !wantsToCrouch && crouchLerp < 0.95f)
                crouchLerp = 0f;
            else if (!grounded && wantsToCrouch)
                crouchLerp = 1f;

            // Collider and position changing
            switch (crouchLerp)
            {
                case > 0.9f when !Crouching:
                {
                    // Begin crouching
                    Crouching = true;
                    if (surfer.Collider.GetType() == typeof(BoxCollider))
                    {
                        // Box collider
                        var boxCollider = (BoxCollider)surfer.Collider;
                        boxCollider.size = new Vector3(
                            boxCollider.size.x,
                            surfer.MoveData.defaultHeight * crouchingHeight,
                            boxCollider.size.z
                        );
                    }
                    else if (surfer.Collider.GetType() == typeof(CapsuleCollider))
                    {
                        // Capsule collider
                        var capsuleCollider = (CapsuleCollider)surfer.Collider;
                        capsuleCollider.height = surfer.MoveData.defaultHeight * crouchingHeight;
                    }

                    // Move position and stuff
                    surfer.MoveData.origin +=
                        heightDifference / 2f * (grounded ? Vector3.down : Vector3.up);
                    foreach (Transform child in playerTransform)
                    {
                        if (child == surfer.MoveData.viewTransform)
                            continue;

                        child.localPosition = new Vector3(
                            child.localPosition.x,
                            child.localPosition.y * crouchingHeight,
                            child.localPosition.z
                        );
                    }

                    UncrouchDown = !grounded;
                    break;
                }

                default:
                    if (Crouching)
                    {
                        // Check if the player can uncrouch
                        var canUncrouch = true;
                        if (surfer.Collider.GetType() == typeof(BoxCollider))
                        {
                            // Box collider
                            var boxCollider = (BoxCollider)surfer.Collider;
                            var halfExtents = boxCollider.size * 0.5f;
                            var startPos = boxCollider.transform.position;
                            var endPos =
                                boxCollider.transform.position
                                + (UncrouchDown ? Vector3.down : Vector3.up) * heightDifference;

                            var trace = Tracer.TraceBox(
                                startPos,
                                endPos,
                                halfExtents,
                                boxCollider.contactOffset,
                                SurfPhysics.groundLayerMask
                            );

                            if (trace.hitCollider != null)
                                canUncrouch = false;
                        }
                        else if (surfer.Collider.GetType() == typeof(CapsuleCollider))
                        {
                            // Capsule collider
                            var capsuleCollider = (CapsuleCollider)surfer.Collider;
                            var point1 =
                                capsuleCollider.center + 0.5f * capsuleCollider.height * Vector3.up;
                            var point2 =
                                capsuleCollider.center
                                + 0.5f * capsuleCollider.height * Vector3.down;
                            var startPos = capsuleCollider.transform.position;
                            var endPos =
                                capsuleCollider.transform.position
                                + (UncrouchDown ? Vector3.down : Vector3.up) * heightDifference;

                            var trace = Tracer.TraceCapsule(
                                point1,
                                point2,
                                capsuleCollider.radius,
                                startPos,
                                endPos,
                                capsuleCollider.contactOffset,
                                SurfPhysics.groundLayerMask
                            );

                            if (trace.hitCollider != null)
                                canUncrouch = false;
                        }

                        // Uncrouch
                        if (canUncrouch && crouchLerp <= 0.9f)
                        {
                            Crouching = false;
                            if (surfer.Collider.GetType() == typeof(BoxCollider))
                            {
                                // Box collider
                                var boxCollider = (BoxCollider)surfer.Collider;
                                boxCollider.size = new Vector3(
                                    boxCollider.size.x,
                                    surfer.MoveData.defaultHeight,
                                    boxCollider.size.z
                                );
                            }
                            else if (surfer.Collider.GetType() == typeof(CapsuleCollider))
                            {
                                // Capsule collider
                                var capsuleCollider = (CapsuleCollider)surfer.Collider;
                                capsuleCollider.height = surfer.MoveData.defaultHeight;
                            }

                            // Move position and stuff
                            surfer.MoveData.origin +=
                                heightDifference / 2f * (UncrouchDown ? Vector3.down : Vector3.up);
                            foreach (Transform child in playerTransform)
                                child.localPosition = new Vector3(
                                    child.localPosition.x,
                                    child.localPosition.y / crouchingHeight,
                                    child.localPosition.z
                                );
                        }

                        if (!canUncrouch)
                            crouchLerp = 1f;
                    }

                    break;
            }

            // Changing camera position
            if (!Crouching)
                surfer.MoveData.viewTransform.localPosition = Vector3.Lerp(
                    surfer.MoveData.viewTransformDefaultLocalPos,
                    surfer.MoveData.viewTransformDefaultLocalPos * crouchingHeight
                        + 0.5f * heightDifference * Vector3.down,
                    crouchLerp
                );
            else
                surfer.MoveData.viewTransform.localPosition = Vector3.Lerp(
                    surfer.MoveData.viewTransformDefaultLocalPos
                        - 0.5f * heightDifference * Vector3.down,
                    surfer.MoveData.viewTransformDefaultLocalPos * crouchingHeight,
                    crouchLerp
                );
        }

        void SlideMovement()
        {
            // Gradually change direction
            slideDirection +=
                deltaTime * slideSpeedCurrent * new Vector3(groundNormal.x, 0f, groundNormal.z);
            slideDirection = slideDirection.normalized;

            // Set direction
            var slideForward = Vector3.Cross(
                groundNormal,
                Quaternion.AngleAxis(-90, Vector3.up) * slideDirection
            );

            // Set the velocity
            slideSpeedCurrent -= config.slideFriction * deltaTime;
            slideSpeedCurrent = Mathf.Clamp(slideSpeedCurrent, 0f, config.maximumSlideSpeed);
            slideSpeedCurrent -=
                (slideForward * slideSpeedCurrent).y
                * deltaTime
                * config.downhillSlideSpeedMultiplier; // Accelerate downhill (-y = downward, - * - = +)

            surfer.MoveData.velocity = slideForward * slideSpeedCurrent;

            // Jump
            if (
                surfer.MoveData.wishJump
                && slideSpeedCurrent < config.minimumSlideSpeed * config.slideSpeedMultiplier
            )
            {
                Jump();
                return;
            }
        }
    }
}
