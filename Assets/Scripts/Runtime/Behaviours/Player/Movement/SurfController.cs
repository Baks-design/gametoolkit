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
        float deltaTime;
        float slideSpeedCurrent = 0f;
        float slideDelay = 0f;
        float crouchLerp = 0f;
        readonly float frictionMult = 1f;
        const float LadderCheckVelocityScale = 2f;
        const float LadderVelocityLerpSpeed = 10f;
        const float GroundCheckDistance = 0.15f;
        const float MaxDistancePerFrame = 0.2f;
        const float CrouchLerpThreshold = 0.9f;
        const float SlopeYVelocityMultiplier = 1.2f;

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
            CacheParameters(surfer, config, deltaTime);

            HandleLadderLogic();

            if (surfer.MoveData.climbingLadder)
                LadderPhysics();
            else if (!surfer.MoveData.underwater)
            {
                HandleAirbornePhysics();
                CheckGrounded();
                CalculateMovementVelocity();
            }
            else
                UnderwaterPhysics();

            ClampAndProcessVelocity();
            ResolveCollisions();
        }

        void CacheParameters(ISurfControllable surfer, MovementConfig config, float deltaTime)
        {
            this.surfer = surfer;
            this.config = config;
            this.deltaTime = deltaTime;
        }

        void HandleLadderLogic()
        {
            if (!surfer.MoveData.laddersEnabled || surfer.MoveData.climbingLadder)
                return;
            var ladderCheckDirection =
                surfer.MoveData.velocity
                * Mathf.Clamp(Time.deltaTime * LadderCheckVelocityScale, 0.025f, 0.25f);
            LadderCheck(new Vector3(1f, 0.95f, 1f), ladderCheckDirection);
        }

        void HandleAirbornePhysics()
        {
            if (surfer.MoveData.velocity.y <= 0f)
                Jumping = false;

            if (surfer.GroundObject == null)
                ApplyGravity();
        }

        void ApplyGravity()
        {
            surfer.MoveData.velocity.y -=
                surfer.MoveData.gravityFactor * config.gravity * deltaTime;
            surfer.MoveData.velocity.y += surfer.BaseVelocity.y * deltaTime;
        }

        void ClampAndProcessVelocity()
        {
            var yVel = surfer.MoveData.velocity.y;
            surfer.MoveData.velocity.y = 0f;
            surfer.MoveData.velocity = Vector3.ClampMagnitude(
                surfer.MoveData.velocity,
                config.maxVelocity
            );
            speed = surfer.MoveData.velocity.magnitude;
            surfer.MoveData.velocity.y = yVel;
        }

        void ResolveCollisions()
        {
            if (surfer.MoveData.velocity.sqrMagnitude == 0f)
                ResolveStaticCollisions();
            else
                ResolveMovingCollisions();
        }

        void ResolveStaticCollisions() =>
            SurfPhysics.ResolveCollisions(
                surfer.Collider,
                ref surfer.MoveData.origin,
                ref surfer.MoveData.velocity,
                surfer.MoveData.rigidbodyPushForce,
                1f,
                surfer.MoveData.stepOffset,
                surfer
            );

        void ResolveMovingCollisions()
        {
            var maxDistPerFrame = MaxDistancePerFrame;
            var velocityThisFrame = surfer.MoveData.velocity * deltaTime;
            var velocityDistLeft = velocityThisFrame.magnitude;
            var initialVel = velocityDistLeft;

            while (velocityDistLeft > 0f)
            {
                var amountThisLoop = Mathf.Min(maxDistPerFrame, velocityDistLeft);
                velocityDistLeft -= amountThisLoop;

                var velThisLoop = velocityThisFrame * (amountThisLoop / initialVel);
                surfer.MoveData.origin += velThisLoop;

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

        void CalculateMovementVelocity()
        {
            if (surfer.MoveType != MoveType.Walk)
                return;

            if (surfer.GroundObject == null)
                HandleAirMovement();
            else
                HandleGroundMovement();
        }

        void HandleAirMovement()
        {
            WasSliding = false;
            surfer.MoveData.velocity += AirInputMovement();
            SurfPhysics.Reflect(
                ref surfer.MoveData.velocity,
                surfer.Collider,
                surfer.MoveData.origin,
                deltaTime
            );
        }

        void HandleGroundMovement()
        {
            InitializeSlideIfNeeded();

            if (ShouldSlide())
            {
                SlideMovement();
                return;
            }

            HandleNormalGroundMovement();
        }

        void InitializeSlideIfNeeded()
        {
            if (WasSliding)
                return;
            slideDirection = new Vector3(
                surfer.MoveData.velocity.x,
                0f,
                surfer.MoveData.velocity.z
            ).normalized;
            slideSpeedCurrent = Mathf.Max(
                config.maximumSlideSpeed,
                new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z).magnitude
            );
        }

        bool ShouldSlide()
        {
            Sliding = false;

            var canSlide =
                surfer.MoveData.velocity.magnitude > config.minimumSlideSpeed
                && surfer.MoveData.slidingEnabled
                && surfer.MoveData.crouching
                && slideDelay <= 0f;

            if (canSlide)
            {
                if (!WasSliding)
                {
                    slideSpeedCurrent = Mathf.Clamp(
                        slideSpeedCurrent * config.slideSpeedMultiplier,
                        config.minimumSlideSpeed,
                        config.maximumSlideSpeed
                    );
                }

                Sliding = true;
                WasSliding = true;
                return true;
            }

            UpdateSlideDelay();
            return false;
        }

        void UpdateSlideDelay()
        {
            if (slideDelay > 0f)
                slideDelay -= deltaTime;
            if (WasSliding)
                slideDelay = config.slideDelay;
            WasSliding = false;
        }

        void HandleNormalGroundMovement()
        {
            var acceleration = Crouching ? config.crouchAcceleration : config.acceleration;

            var forward = Vector3.Cross(groundNormal, -playerTransform.right);
            var right = Vector3.Cross(groundNormal, forward);

            var speed = GetMovementSpeed();

            if (surfer.MoveData.wishJump)
            {
                ApplyFriction(0f, true, true);
                Jump();
                return;
            }

            ApplyFriction(1f * frictionMult, true, true);

            Vector3 wishDir = CalculateWishDirection(forward, right);
            wishDir.Normalize();

            var wishSpeed = wishDir.magnitude * speed;

            var yVel = surfer.MoveData.velocity.y;
            Accelerate(wishDir, wishSpeed, acceleration * Mathf.Min(frictionMult, 1f), false);

            surfer.MoveData.velocity = Vector3.ClampMagnitude(
                new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z),
                config.maxVelocity
            );
            surfer.MoveData.velocity.y = yVel;

            ApplySlopeMovement(wishDir);
        }

        float GetMovementSpeed()
        {
            if (Crouching)
                return config.crouchSpeed;
            return surfer.MoveData.sprinting ? config.sprintSpeed : config.walkSpeed;
        }

        Vector3 CalculateWishDirection(Vector3 forward, Vector3 right) =>
            surfer.MoveData.verticalAxis * forward + surfer.MoveData.horizontalAxis * right;

        void ApplySlopeMovement(Vector3 wishDir)
        {
            var forwardVelocity = Vector3.Cross(
                groundNormal,
                Quaternion.AngleAxis(-90f, Vector3.up)
                    * new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z)
            );

            var yVelocityNew =
                forwardVelocity.normalized.y
                * new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z).magnitude;

            surfer.MoveData.velocity.y =
                yVelocityNew * (wishDir.y < 0f ? SlopeYVelocityMultiplier : 1f);
        }

        void UnderwaterPhysics()
        {
            ApplyUnderwaterDamping();

            if (!CheckGrounded())
                ApplyUnderwaterGravity();

            HandleSwimmingInput();

            var acceleration = config.underwaterAcceleration;

            ApplyFriction(1f, true, false);

            var forward = Vector3.Cross(groundNormal, -playerTransform.right);
            var right = Vector3.Cross(groundNormal, forward);

            var speed = config.underwaterSwimSpeed;
            var wishDir = CalculateWishDirection(forward, right);
            wishDir.Normalize();

            var wishSpeed = wishDir.magnitude * speed;

            var yVel = surfer.MoveData.velocity.y;
            Accelerate(wishDir, wishSpeed, acceleration, false);

            surfer.MoveData.velocity = Vector3.ClampMagnitude(
                new Vector3(surfer.MoveData.velocity.x, 0f, surfer.MoveData.velocity.z),
                config.maxVelocity
            );
            surfer.MoveData.velocity.y = yVel;

            HandleWaterJump();
        }

        void ApplyUnderwaterDamping() =>
            surfer.MoveData.velocity = Vector3.Lerp(
                surfer.MoveData.velocity,
                Vector3.zero,
                config.underwaterVelocityDampening * deltaTime
            );

        void ApplyUnderwaterGravity() =>
            surfer.MoveData.velocity.y -= config.underwaterGravity * deltaTime;

        void HandleSwimmingInput()
        {
            if (Input.GetButton("Jump"))
                surfer.MoveData.velocity.y += config.swimUpSpeed * deltaTime;
        }

        void HandleWaterJump()
        {
            var movingForwards =
                playerTransform.InverseTransformVector(surfer.MoveData.velocity).z > 0f;
            var waterJumpTrace = TraceBounds(
                playerTransform.position,
                playerTransform.position + playerTransform.forward * 0.1f,
                SurfPhysics.groundLayerMask
            );

            var canWaterJump =
                waterJumpTrace.hitCollider != null
                && Vector3.Angle(Vector3.up, waterJumpTrace.planeNormal) >= config.slopeLimit
                && Input.GetButton("Jump")
                && !surfer.MoveData.cameraUnderwater
                && movingForwards;
            if (canWaterJump)
            {
                surfer.MoveData.velocity.y = Mathf.Max(
                    surfer.MoveData.velocity.y,
                    config.jumpForce
                );
            }
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
                if (ladder != null && CanClimbLadder(rays[i]))
                {
                    foundLadder = true;
                    InitializeLadderClimb(rays[i], direction);
                    break;
                }
            }

            if (!foundLadder)
                ResetLadderState();
        }

        bool CanClimbLadder(RaycastHit rayHit)
        {
            if (!surfer.MoveData.angledLaddersEnabled)
                return rayHit.normal.y == 0f;
            if (rayHit.normal.y < 0f)
                return false;
            var ladderAngle = Vector3.Angle(Vector3.up, rayHit.normal);
            return ladderAngle > surfer.MoveData.slopeLimit;
        }

        void InitializeLadderClimb(RaycastHit rayHit, Vector3 direction)
        {
            if (!surfer.MoveData.climbingLadder)
            {
                surfer.MoveData.climbingLadder = true;
                surfer.MoveData.ladderNormal = rayHit.normal;
                surfer.MoveData.ladderDirection = 2f * direction.magnitude * -rayHit.normal;

                if (surfer.MoveData.angledLaddersEnabled)
                {
                    var sideDir = rayHit.normal;
                    sideDir.y = 0f;
                    sideDir = Quaternion.AngleAxis(-90f, Vector3.up) * sideDir;

                    surfer.MoveData.ladderClimbDir =
                        Quaternion.AngleAxis(90f, sideDir) * rayHit.normal;
                    surfer.MoveData.ladderClimbDir *= 1f / surfer.MoveData.ladderClimbDir.y;
                }
                else
                    surfer.MoveData.ladderClimbDir = Vector3.up;
            }
        }

        void ResetLadderState()
        {
            surfer.MoveData.ladderNormal = Vector3.zero;
            surfer.MoveData.ladderVelocity = Vector3.zero;
            surfer.MoveData.climbingLadder = false;
            surfer.MoveData.ladderClimbDir = Vector3.up;
        }

        void LadderPhysics()
        {
            surfer.MoveData.ladderVelocity =
                6f * surfer.MoveData.verticalAxis * surfer.MoveData.ladderClimbDir;
            surfer.MoveData.velocity = Vector3.Lerp(
                surfer.MoveData.velocity,
                surfer.MoveData.ladderVelocity,
                Time.deltaTime * LadderVelocityLerpSpeed
            );
            LadderCheck(Vector3.one, surfer.MoveData.ladderDirection);
            HandleLadderExit();
        }

        void HandleLadderExit()
        {
            var floorTrace = TraceToFloor();
            var shouldExitLadder =
                surfer.MoveData.verticalAxis < 0f
                && floorTrace.hitCollider != null
                && Vector3.Angle(Vector3.up, floorTrace.planeNormal) <= surfer.MoveData.slopeLimit;
            if (shouldExitLadder)
                ExitLadder();

            if (surfer.MoveData.wishJump)
                JumpOffLadder();
        }

        void ExitLadder()
        {
            surfer.MoveData.velocity = surfer.MoveData.ladderNormal * 0.5f;
            surfer.MoveData.ladderVelocity = Vector3.zero;
            surfer.MoveData.climbingLadder = false;
        }

        void JumpOffLadder()
        {
            surfer.MoveData.velocity = surfer.MoveData.ladderNormal * 4f;
            surfer.MoveData.ladderVelocity = Vector3.zero;
            surfer.MoveData.climbingLadder = false;
        }

        void Accelerate(Vector3 wishDir, float wishSpeed, float acceleration, bool yMovement)
        {
            var currentSpeed = Vector3.Dot(surfer.MoveData.velocity, wishDir);
            var addSpeed = wishSpeed - currentSpeed;
            if (addSpeed <= 0f)
                return;

            var accelerationSpeed = Mathf.Min(acceleration * deltaTime * wishSpeed, addSpeed);
            surfer.MoveData.velocity.x += accelerationSpeed * wishDir.x;
            if (yMovement)
                surfer.MoveData.velocity.y += accelerationSpeed * wishDir.y;
            surfer.MoveData.velocity.z += accelerationSpeed * wishDir.z;
        }

        void ApplyFriction(float t, bool yAffected, bool grounded)
        {
            var vel = surfer.MoveData.velocity;
            vel.y = 0f;
            speed = vel.magnitude;
            var drop = 0f;

            var friction = Crouching ? config.crouchFriction : config.friction;
            var deceleration = Crouching ? config.crouchDeceleration : config.deceleration;

            if (grounded)
            {
                vel.y = surfer.MoveData.velocity.y;
                var control = speed < deceleration ? deceleration : speed;
                drop = control * friction * deltaTime * t;
            }

            var newSpeed = Mathf.Max(speed - drop, 0f);
            if (speed > 0f)
                newSpeed /= speed;

            surfer.MoveData.velocity.x *= newSpeed;
            if (yAffected)
                surfer.MoveData.velocity.y *= newSpeed;
            surfer.MoveData.velocity.z *= newSpeed;
        }

        Vector3 AirInputMovement()
        {
            GetWishValues(out var wishVel, out var wishDir, out var wishSpeed);

            if (config.clampAirSpeed && wishSpeed > 0f && wishSpeed > config.maxSpeed)
                wishSpeed = config.maxSpeed;

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
            var forward = surfer.Forward;
            forward.y = 0f;
            forward.Normalize();

            var right = surfer.Right;
            right.y = 0f;
            right.Normalize();

            wishVel = forward * surfer.MoveData.forwardMove + right * surfer.MoveData.sideMove;
            wishVel.y = 0f;

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

            var isGrounded =
                trace.hitCollider != null
                && groundSteepness <= config.slopeLimit
                && !(Jumping && surfer.MoveData.velocity.y > 0f);
            if (isGrounded)
            {
                groundNormal = trace.planeNormal;
                SetGround(trace.hitCollider.gameObject);
                return true;
            }
            else
            {
                SetGround(null);
                if (movingUp && surfer.MoveType != MoveType.Noclip)
                    surfer.MoveData.surfaceFriction = config.airFriction;
                return false;
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
            down.y -= GroundCheckDistance;
            return Tracer.TraceCollider(
                surfer.Collider,
                surfer.MoveData.origin,
                down,
                SurfPhysics.groundLayerMask
            );
        }

        public void Crouch(ISurfControllable surfer, MovementConfig config, float deltaTime)
        {
            CacheParameters(surfer, config, deltaTime);
            if (surfer == null || surfer.Collider == null)
                return;

            var grounded = surfer.GroundObject != null;
            var wantsToCrouch = surfer.MoveData.crouching;
            UpdateCrouchLerp(grounded, wantsToCrouch);
            HandleCrouchState(grounded);
            UpdateCameraPosition();
        }

        void UpdateCrouchLerp(bool grounded, bool wantsToCrouch)
        {
            if (grounded)
            {
                UncrouchDown = false;
                crouchLerp = Mathf.Lerp(
                    crouchLerp,
                    wantsToCrouch ? 1f : 0f,
                    deltaTime * surfer.MoveData.crouchingSpeed
                );
            }
            else if (!grounded && !wantsToCrouch && crouchLerp < 0.95f)
                crouchLerp = 0f;
            else if (!grounded && wantsToCrouch)
                crouchLerp = 1f;
        }

        void HandleCrouchState(bool grounded)
        {
            if (crouchLerp > CrouchLerpThreshold && !Crouching)
                BeginCrouching(grounded);
            else if (Crouching)
                TryUncrouch();
        }

        void BeginCrouching(bool grounded)
        {
            Crouching = true;
            AdjustColliderSize();
            AdjustPositionAndChildren(grounded);
            UncrouchDown = !grounded;
        }

        void AdjustColliderSize()
        {
            var crouchingHeight = Mathf.Clamp(surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var targetHeight = surfer.MoveData.defaultHeight * crouchingHeight;

            switch (surfer.Collider)
            {
                case BoxCollider boxCollider:
                    boxCollider.size = new Vector3(
                        boxCollider.size.x,
                        targetHeight,
                        boxCollider.size.z
                    );
                    break;
                case CapsuleCollider capsuleCollider:
                    capsuleCollider.height = targetHeight;
                    break;
            }
        }

        void AdjustPositionAndChildren(bool grounded)
        {
            var crouchingHeight = Mathf.Clamp(surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var heightDifference =
                surfer.MoveData.defaultHeight - surfer.MoveData.defaultHeight * crouchingHeight;

            surfer.MoveData.origin +=
                heightDifference * 0.5f * (grounded ? Vector3.down : Vector3.up);

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
        }

        void TryUncrouch()
        {
            var canUncrouch = CheckUncrouchSpace();
            if (canUncrouch && crouchLerp <= CrouchLerpThreshold)
                EndCrouching();
            else if (!canUncrouch)
                crouchLerp = 1f;
        }

        bool CheckUncrouchSpace()
        {
            var crouchingHeight = Mathf.Clamp(surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var heightDifference =
                surfer.MoveData.defaultHeight - surfer.MoveData.defaultHeight * crouchingHeight;

            var direction = UncrouchDown ? Vector3.down : Vector3.up;
            var startPos = surfer.Collider.transform.position;
            var endPos = startPos + direction * heightDifference;

            return surfer.Collider switch
            {
                BoxCollider boxCollider => CheckBoxUncrouchSpace(boxCollider, startPos, endPos),
                CapsuleCollider capsuleCollider
                    => CheckCapsuleUncrouchSpace(capsuleCollider, startPos, endPos),
                _ => true,
            };
        }

        bool CheckBoxUncrouchSpace(BoxCollider boxCollider, Vector3 startPos, Vector3 endPos)
        {
            var halfExtents = boxCollider.size * 0.5f;
            var trace = Tracer.TraceBox(
                startPos,
                endPos,
                halfExtents,
                boxCollider.contactOffset,
                SurfPhysics.groundLayerMask
            );
            return trace.hitCollider == null;
        }

        bool CheckCapsuleUncrouchSpace(
            CapsuleCollider capsuleCollider,
            Vector3 startPos,
            Vector3 endPos
        )
        {
            var point1 = capsuleCollider.center + 0.5f * capsuleCollider.height * Vector3.up;
            var point2 = capsuleCollider.center + 0.5f * capsuleCollider.height * Vector3.down;
            var trace = Tracer.TraceCapsule(
                point1,
                point2,
                capsuleCollider.radius,
                startPos,
                endPos,
                capsuleCollider.contactOffset,
                SurfPhysics.groundLayerMask
            );
            return trace.hitCollider == null;
        }

        void EndCrouching()
        {
            Crouching = false;
            ResetColliderSize();
            ResetPositionAndChildren();
        }

        void ResetColliderSize()
        {
            switch (surfer.Collider)
            {
                case BoxCollider boxCollider:
                    boxCollider.size = new Vector3(
                        boxCollider.size.x,
                        surfer.MoveData.defaultHeight,
                        boxCollider.size.z
                    );
                    break;
                case CapsuleCollider capsuleCollider:
                    capsuleCollider.height = surfer.MoveData.defaultHeight;
                    break;
            }
        }

        void ResetPositionAndChildren()
        {
            var crouchingHeight = Mathf.Clamp(surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var heightDifference =
                surfer.MoveData.defaultHeight - surfer.MoveData.defaultHeight * crouchingHeight;

            surfer.MoveData.origin +=
                heightDifference * 0.5f * (UncrouchDown ? Vector3.down : Vector3.up);

            foreach (Transform child in playerTransform)
                child.localPosition = new Vector3(
                    child.localPosition.x,
                    child.localPosition.y / crouchingHeight,
                    child.localPosition.z
                );
        }

        void UpdateCameraPosition()
        {
            var crouchingHeight = Mathf.Clamp(surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var heightDifference =
                surfer.MoveData.defaultHeight - surfer.MoveData.defaultHeight * crouchingHeight;

            var targetPosition = Crouching
                ? surfer.MoveData.viewTransformDefaultLocalPos * crouchingHeight
                : surfer.MoveData.viewTransformDefaultLocalPos
                    - 0.5f * heightDifference * Vector3.down;

            var startPosition = Crouching
                ? surfer.MoveData.viewTransformDefaultLocalPos
                    - 0.5f * heightDifference * Vector3.down
                : surfer.MoveData.viewTransformDefaultLocalPos * crouchingHeight
                    + 0.5f * heightDifference * Vector3.down;

            surfer.MoveData.viewTransform.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                crouchLerp
            );
        }

        void SlideMovement()
        {
            UpdateSlideDirection();
            ApplySlideMovement();
            HandleSlideJump();
        }

        void UpdateSlideDirection()
        {
            slideDirection +=
                deltaTime * slideSpeedCurrent * new Vector3(groundNormal.x, 0f, groundNormal.z);
            slideDirection = slideDirection.normalized;
        }

        void ApplySlideMovement()
        {
            var slideForward = Vector3.Cross(
                groundNormal,
                Quaternion.AngleAxis(-90f, Vector3.up) * slideDirection
            );

            slideSpeedCurrent -= config.slideFriction * deltaTime;
            slideSpeedCurrent = Mathf.Clamp(slideSpeedCurrent, 0f, config.maximumSlideSpeed);

            // Accelerate downhill
            var downhillAcceleration =
                (slideForward * slideSpeedCurrent).y
                * deltaTime
                * config.downhillSlideSpeedMultiplier;
            slideSpeedCurrent -= downhillAcceleration;

            surfer.MoveData.velocity = slideForward * slideSpeedCurrent;
        }

        void HandleSlideJump()
        {
            var canJump =
                surfer.MoveData.wishJump
                && slideSpeedCurrent < config.minimumSlideSpeed * config.slideSpeedMultiplier;
            if (canJump)
                Jump();
        }
    }
}
