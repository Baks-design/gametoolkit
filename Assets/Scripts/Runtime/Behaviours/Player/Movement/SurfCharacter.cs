using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    /// <summary>
    /// Easily add a surfable character to the scene
    /// </summary>
    [AddComponentMenu("Fragsurf/Surf Character")]
    public class SurfCharacter : MonoBehaviour, ISurfControllable
    {
        public enum ColliderType
        {
            Capsule,
            Box
        }

        [Header("Physics Settings")]
        public Vector3 colliderSize = new(1f, 2f, 1f);

        // Capsule doesn't work anymore; I'll have to figure out why some other time, sorry.
        public float weight = 75f;
        public float rigidbodyPushForce = 2f;
        public bool solidCollider = false;

        [Header("View Settings")]
        public Transform viewTransform;
        public Transform playerRotationTransform;

        [Header("Crouching setup")]
        public float crouchingHeightMultiplier = 0.5f;
        public float crouchingSpeed = 10f;
        float defaultHeight;

        // This is separate because you shouldn't be able to toggle crouching on and off during gameplay for various reasons
        bool allowCrouch = true;

        [Header("Features")]
        public bool crouchingEnabled = true;
        public bool slidingEnabled = false;
        public bool laddersEnabled = true;
        public bool supportAngledLadders = true;

        [Header("Step offset (can be buggy, enable at your own risk)")]
        public bool useStepOffset = false;
        public float stepOffset = 0.35f;

        readonly SurfController controller = new();
        readonly List<Collider> triggers = new();
        CameraWaterCheck cameraWaterCheck;
        Rigidbody rb;
        GameObject colliderObject;
        GameObject cameraWaterCheckObject;
        Vector3 baseVelocity;
        Vector3 angles;
        Vector3 prevPosition;
        int numberOfTriggers = 0;
        bool underwater = false;

        [Header("Movement Config")]
        [SerializeField]
        MovementConfig MoveConfig;

        public MoveData MoveData { get; } = new();
        public Vector3 StartPosition { get; private set; }
        public Collider Collider { get; private set; }
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
            Gizmos.DrawWireCube(transform.position, colliderSize);
        }

        void Awake()
        {
            controller.playerTransform = playerRotationTransform;
            if (viewTransform != null)
            {
                controller.camera = viewTransform;
                controller.cameraYPos = viewTransform.localPosition.y;
            }
        }

        void Start()
        {
            colliderObject = new GameObject("PlayerCollider") { layer = gameObject.layer };
            colliderObject.transform.SetParent(transform);
            colliderObject.transform.rotation = Quaternion.identity;
            colliderObject.transform.localPosition = Vector3.zero;
            colliderObject.transform.SetSiblingIndex(0);

            // Water check
            cameraWaterCheckObject = new GameObject("Camera water check")
            {
                layer = gameObject.layer
            };
            cameraWaterCheckObject.transform.position = viewTransform.position;

            var cameraWaterCheckSphere = cameraWaterCheckObject.AddComponent<SphereCollider>();
            cameraWaterCheckSphere.radius = 0.1f;
            cameraWaterCheckSphere.isTrigger = true;

            var cameraWaterCheckRb = cameraWaterCheckObject.AddComponent<Rigidbody>();
            cameraWaterCheckRb.useGravity = false;
            cameraWaterCheckRb.isKinematic = true;
            cameraWaterCheck = cameraWaterCheckObject.AddComponent<CameraWaterCheck>();

            prevPosition = transform.position;

            if (viewTransform == null)
                viewTransform = Camera.main.transform;

            if (playerRotationTransform == null && transform.childCount > 0)
                playerRotationTransform = transform.GetChild(0);

            Collider = gameObject.GetComponent<Collider>();

            if (Collider != null)
                Destroy(Collider);

            // rigidbody is required to collide with triggers
            gameObject.TryGetComponent(out rb);
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();

            allowCrouch = crouchingEnabled;

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.angularDamping = 0f;
            rb.linearDamping = 0f;
            rb.mass = weight;

            switch (CollisionType)
            {
                // Box collider
                case ColliderType.Box:
                    Collider = colliderObject.AddComponent<BoxCollider>();
                    var boxc = (BoxCollider)Collider;
                    boxc.size = colliderSize;
                    defaultHeight = boxc.size.y;
                    break;

                // Capsule collider
                case ColliderType.Capsule:
                    Collider = colliderObject.AddComponent<CapsuleCollider>();
                    var capc = (CapsuleCollider)Collider;
                    capc.height = colliderSize.y;
                    capc.radius = colliderSize.x / 2f;
                    defaultHeight = capc.height;
                    break;
            }

            MoveData.slopeLimit = MoveConfig.slopeLimit;
            MoveData.rigidbodyPushForce = rigidbodyPushForce;
            MoveData.slidingEnabled = slidingEnabled;
            MoveData.laddersEnabled = laddersEnabled;
            MoveData.angledLaddersEnabled = supportAngledLadders;
            MoveData.playerTransform = transform;
            MoveData.viewTransform = viewTransform;
            MoveData.viewTransformDefaultLocalPos = viewTransform.localPosition;
            MoveData.defaultHeight = defaultHeight;
            MoveData.crouchingHeight = crouchingHeightMultiplier;
            MoveData.crouchingSpeed = crouchingSpeed;
            MoveData.origin = transform.position;
            MoveData.useStepOffset = useStepOffset;
            MoveData.stepOffset = stepOffset;

            Collider.isTrigger = !solidCollider;
            StartPosition = transform.position;
        }

        void Update()
        {
            colliderObject.transform.rotation = Quaternion.identity;

            UpdateMoveData();

            // Previous movement code
            var positionalMovement = transform.position - prevPosition;
            transform.position = prevPosition;
            MoveData.origin += positionalMovement;

            // Triggers
            if (numberOfTriggers != triggers.Count)
            {
                numberOfTriggers = triggers.Count;

                underwater = false;
                triggers.RemoveAll(item => item == null);
                foreach (var trigger in triggers)
                {
                    if (trigger == null)
                        continue;

                    if (trigger.GetComponentInParent<Water>())
                        underwater = true;
                }
            }

            MoveData.cameraUnderwater = cameraWaterCheck.IsUnderwater();
            cameraWaterCheckObject.transform.position = viewTransform.position;
            MoveData.underwater = underwater;

            if (allowCrouch)
                controller.Crouch(this, MoveConfig, Time.deltaTime);

            controller.ProcessMovement(this, MoveConfig, Time.deltaTime);

            transform.position = MoveData.origin;
            prevPosition = transform.position;

            colliderObject.transform.rotation = Quaternion.identity;
        }

        void UpdateMoveData()
        {
            MoveData.verticalAxis = Input.GetAxisRaw("Vertical");
            MoveData.horizontalAxis = Input.GetAxisRaw("Horizontal");
            MoveData.sprinting = Input.GetButton("Sprint");

            if (Input.GetButtonDown("Crouch"))
                MoveData.crouching = true;
            if (!Input.GetButton("Crouch"))
                MoveData.crouching = false;

            var moveLeft = MoveData.horizontalAxis < 0f;
            var moveRight = MoveData.horizontalAxis > 0f;
            var moveFwd = MoveData.verticalAxis > 0f;
            var moveBack = MoveData.verticalAxis < 0f;

            if (!moveLeft && !moveRight)
                MoveData.sideMove = 0f;
            else if (moveLeft)
                MoveData.sideMove = -MoveConfig.acceleration;
            else if (moveRight)
                MoveData.sideMove = MoveConfig.acceleration;

            if (!moveFwd && !moveBack)
                MoveData.forwardMove = 0f;
            else if (moveFwd)
                MoveData.forwardMove = MoveConfig.acceleration;
            else if (moveBack)
                MoveData.forwardMove = -MoveConfig.acceleration;

            if (Input.GetButtonDown("Jump"))
                MoveData.wishJump = true;

            if (!Input.GetButton("Jump"))
                MoveData.wishJump = false;

            MoveData.viewAngles = angles;
        }

        public static float ClampAngle(float angle, float from, float to)
        {
            if (angle < 0f)
                angle = 360 + angle;
            if (angle > 180f)
                return Mathf.Max(angle, 360f + from);
            return Mathf.Min(angle, to);
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

        void OnCollisionStay(Collision collision)
        {
            if (collision.rigidbody == null)
                return;

            var relativeVelocity = collision.relativeVelocity * collision.rigidbody.mass / 50f;
            var impactVelocity = new Vector3(
                relativeVelocity.x * 0.0025f,
                relativeVelocity.y * 0.00025f,
                relativeVelocity.z * 0.0025f
            );

            var maxYVel = Mathf.Max(MoveData.velocity.y, 10f);
            var newVelocity = new Vector3(
                MoveData.velocity.x + impactVelocity.x,
                Mathf.Clamp(
                    MoveData.velocity.y + Mathf.Clamp(impactVelocity.y, -0.5f, 0.5f),
                    -maxYVel,
                    maxYVel
                ),
                MoveData.velocity.z + impactVelocity.z
            );

            newVelocity = Vector3.ClampMagnitude(
                newVelocity,
                Mathf.Max(MoveData.velocity.magnitude, 30f)
            );
            MoveData.velocity = newVelocity;
        }
    }
}
