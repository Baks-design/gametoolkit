using System;
using System.Collections.Generic;
using System.Linq;
using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameToolkit.Runtime.Behaviours.Player
{
    /// <summary>
    /// Easily add a surfable character to the scene
    /// </summary>
    [AddComponentMenu("Fragsurf/Surf Character")]
    public class SurfCharacter : CustomMonoBehaviour, ISurfControllable, IUpdatable
    {
        public enum ColliderType
        {
            Capsule,
            Box
        }

        [Header("Physics Settings")]
        public Vector3 colliderSize = new(1f, 2f, 1f);
        public float weight = 75f;
        public float rigidbodyPushForce = 2f;
        public bool solidCollider = false;

        [Header("View Settings")]
        public Transform viewTransform;
        public Transform playerRotationTransform;

        [Header("Crouching Setup")]
        public float crouchingHeightMultiplier = 0.5f;
        public float crouchingSpeed = 10f;

        [Header("Features")]
        public bool crouchingEnabled = true;
        public bool slidingEnabled = false;
        public bool laddersEnabled = true;
        public bool supportAngledLadders = true;

        [Header("Step Offset (can be buggy, enable at your own risk)")]
        public bool useStepOffset = false;
        public float stepOffset = 0.35f;

        [Header("Movement Config")]
        [SerializeField]
        MovementConfig MoveConfig;

        readonly SurfController controller = new();
        readonly List<Collider> triggers = new();
        CameraWaterCheck cameraWaterCheck;
        Rigidbody rb;
        GameObject colliderObject;
        GameObject cameraWaterCheckObject;
        Vector3 baseVelocity;
        Vector3 prevPosition;
        int numberOfTriggers = 0;
        float defaultHeight;
        bool underwater = false;
        bool allowCrouch = true;
        const float ImpactVelocityXZMultiplier = 0.0025f;
        const float ImpactVelocityYMultiplier = 0.00025f;
        const float MaxImpactY = 0.5f;
        const float MaxVelocityClamp = 30f;
        const float CameraWaterCheckRadius = 0.1f;

        public MoveData MoveData { get; } = new();
        public Vector3 StartPosition { get; set; }
        public Collider Collider { get; set; }
        public GameObject GroundObject { get; set; }
        public ColliderType CollisionType => ColliderType.Box;
        public MoveType MoveType => MoveType.Walk;
        public Vector3 BaseVelocity => baseVelocity;
        public Vector3 Forward => viewTransform.forward;
        public Vector3 Right => viewTransform.right;
        public Vector3 Up => viewTransform.up;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Transform.position, colliderSize);
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeController();
        }

        protected override void Start()
        {
            InitializeCollider();
            InitializeWaterCheck();
            InitializeRigidbody();
            InitializeMoveData();
            StartPosition = Transform.position;
        }

        public override void ProcessUpdate(float deltaTime)
        {
            UpdateColliderRotation();
            UpdateMoveData();
            ProcessPositionalMovement();
            UpdateTriggers();
            UpdateWaterStatus();
            ProcessCrouching(deltaTime);
            ProcessMovement(deltaTime);
            UpdateTransformPosition();
        }

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

        void OnCollisionStay(Collision collision) => HandleRigidbodyCollision(collision);

        void InitializeController()
        {
            controller.playerTransform = playerRotationTransform;
            if (viewTransform != null)
            {
                controller.camera = viewTransform;
                controller.cameraYPos = viewTransform.localPosition.y;
            }
        }

        void InitializeCollider()
        {
            colliderObject = new GameObject("PlayerCollider") { layer = gameObject.layer };
            colliderObject.transform.SetParent(Transform);
            colliderObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            colliderObject.transform.SetSiblingIndex(0);

            // Remove existing collider if present
            if (TryGetComponent<Collider>(out var existingCollider))
                Destroy(existingCollider);

            Collider = CreateColliderByType();
            Collider.isTrigger = !solidCollider;
        }

        Collider CreateColliderByType() =>
            CollisionType switch
            {
                ColliderType.Box => CreateBoxCollider(),
                ColliderType.Capsule => CreateCapsuleCollider(),
                _
                    => throw new NotImplementedException(
                        $"Collider type {CollisionType} not implemented"
                    )
            };

        BoxCollider CreateBoxCollider()
        {
            var boxCollider = colliderObject.AddComponent<BoxCollider>();
            boxCollider.size = colliderSize;
            defaultHeight = boxCollider.size.y;
            return boxCollider;
        }

        CapsuleCollider CreateCapsuleCollider()
        {
            var capsuleCollider = colliderObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = colliderSize.y;
            capsuleCollider.radius = colliderSize.x * 0.5f;
            defaultHeight = capsuleCollider.height;
            return capsuleCollider;
        }

        void InitializeWaterCheck()
        {
            cameraWaterCheckObject = new GameObject("Camera water check")
            {
                layer = gameObject.layer
            };
            cameraWaterCheckObject.transform.position = viewTransform.position;

            var cameraWaterCheckSphere = cameraWaterCheckObject.AddComponent<SphereCollider>();
            cameraWaterCheckSphere.radius = CameraWaterCheckRadius;
            cameraWaterCheckSphere.isTrigger = true;

            var cameraWaterCheckRb = cameraWaterCheckObject.AddComponent<Rigidbody>();
            cameraWaterCheckRb.useGravity = false;
            cameraWaterCheckRb.isKinematic = true;
            cameraWaterCheck = cameraWaterCheckObject.AddComponent<CameraWaterCheck>();

            EnsureViewTransform();
        }

        void EnsureViewTransform()
        {
            if (viewTransform == null)
                viewTransform = Camera.main.transform;

            if (playerRotationTransform == null && Transform.childCount > 0)
                playerRotationTransform = Transform.GetChild(0);
        }

        void InitializeRigidbody()
        {
            if (!TryGetComponent(out rb))
                rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.angularDamping = 0f;
            rb.linearDamping = 0f;
            rb.mass = weight;
        }

        void InitializeMoveData()
        {
            allowCrouch = crouchingEnabled;

            MoveData.slopeLimit = MoveConfig.slopeLimit;
            MoveData.rigidbodyPushForce = rigidbodyPushForce;
            MoveData.slidingEnabled = slidingEnabled;
            MoveData.laddersEnabled = laddersEnabled;
            MoveData.angledLaddersEnabled = supportAngledLadders;
            MoveData.playerTransform = Transform;
            MoveData.viewTransform = viewTransform;
            MoveData.viewTransformDefaultLocalPos = viewTransform.localPosition;
            MoveData.defaultHeight = defaultHeight;
            MoveData.crouchingHeight = crouchingHeightMultiplier;
            MoveData.crouchingSpeed = crouchingSpeed;
            MoveData.origin = Transform.position;
            MoveData.useStepOffset = useStepOffset;
            MoveData.stepOffset = stepOffset;
        }

        void UpdateColliderRotation() => colliderObject.transform.rotation = Quaternion.identity;

        void UpdateMoveData()
        {
            UpdateMoveInput();
            UpdateSprintInput();
            UpdateCrouchInput();
            UpdateMovementInput();
            UpdateJumpInput();
        }

        void UpdateMoveInput()
        {
            if (Keyboard.current.wKey.isPressed)
                MoveData.verticalAxis = 1f;
            if (Keyboard.current.sKey.isPressed)
                MoveData.verticalAxis = -1f;
            if (Keyboard.current.dKey.isPressed)
                MoveData.horizontalAxis = 1f;
            if (Keyboard.current.aKey.isPressed)
                MoveData.horizontalAxis = -1f;
        }

        void UpdateSprintInput() => MoveData.sprinting = Keyboard.current.leftShiftKey.isPressed;

        void UpdateCrouchInput()
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
                MoveData.crouching = true;
            if (!Keyboard.current.cKey.isPressed)
                MoveData.crouching = false;
        }

        void UpdateMovementInput()
        {
            MoveData.sideMove = CalculateSideMovement();
            MoveData.forwardMove = CalculateForwardMovement();
        }

        float CalculateSideMovement() =>
            MoveData.horizontalAxis switch
            {
                < 0f => -MoveConfig.acceleration,
                > 0f => MoveConfig.acceleration,
                _ => 0f
            };

        float CalculateForwardMovement() =>
            MoveData.verticalAxis switch
            {
                > 0f => MoveConfig.acceleration,
                < 0f => -MoveConfig.acceleration,
                _ => 0f
            };

        void UpdateJumpInput()
        {
            MoveData.wishJump = Keyboard.current.spaceKey.wasPressedThisFrame;
            if (!Keyboard.current.spaceKey.isPressed)
                MoveData.wishJump = false;
        }

        void ProcessPositionalMovement()
        {
            var positionalMovement = Transform.position - prevPosition;
            Transform.position = prevPosition;
            MoveData.origin += positionalMovement;
        }

        void UpdateTriggers()
        {
            if (numberOfTriggers == triggers.Count)
                return;
            numberOfTriggers = triggers.Count;
            triggers.RemoveAll(item => item == null);
            UpdateUnderwaterStatus();
        }

        void UpdateUnderwaterStatus() =>
            underwater = triggers.Any(trigger =>
                trigger != null && trigger.GetComponentInParent<Water>() != null
            );

        void UpdateWaterStatus()
        {
            MoveData.cameraUnderwater = cameraWaterCheck.IsUnderwater();
            cameraWaterCheckObject.transform.position = viewTransform.position;
            MoveData.underwater = underwater;
        }

        void ProcessCrouching(float deltaTime)
        {
            if (allowCrouch)
                controller.Crouch(this, MoveConfig, deltaTime);
        }

        void ProcessMovement(float deltaTime) =>
            controller.ProcessMovement(this, MoveConfig, deltaTime);

        void UpdateTransformPosition()
        {
            Transform.position = MoveData.origin;
            prevPosition = transform.position;
            colliderObject.transform.rotation = Quaternion.identity;
        }

        void HandleRigidbodyCollision(Collision collision)
        {
            if (collision.rigidbody == null)
                return;
            var relativeVelocity = collision.relativeVelocity * (collision.rigidbody.mass / 50f);
            var impactVelocity = CalculateImpactVelocity(relativeVelocity);
            var maxYVel = Mathf.Max(MoveData.velocity.y, 10f);
            var newVelocity = CalculateNewVelocity(impactVelocity, maxYVel);
            MoveData.velocity = Vector3.ClampMagnitude(
                newVelocity,
                Mathf.Max(MoveData.velocity.magnitude, MaxVelocityClamp)
            );
        }

        Vector3 CalculateImpactVelocity(Vector3 relativeVelocity) =>
            new(
                relativeVelocity.x * ImpactVelocityXZMultiplier,
                Mathf.Clamp(
                    relativeVelocity.y * ImpactVelocityYMultiplier,
                    -MaxImpactY,
                    MaxImpactY
                ),
                relativeVelocity.z * ImpactVelocityXZMultiplier
            );

        Vector3 CalculateNewVelocity(Vector3 impactVelocity, float maxYVel) =>
            new(
                MoveData.velocity.x + impactVelocity.x,
                Mathf.Clamp(MoveData.velocity.y + impactVelocity.y, -maxYVel, maxYVel),
                MoveData.velocity.z + impactVelocity.z
            );
    }
}
