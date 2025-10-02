using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerController : StatefulEntity, IFixedUpdatable, IUpdatable, ILateUpdatable
    {
        [SerializeField, HideInInspector]
        PlayerCollisionData collisionData;
        IFixedUpdateServices fixedUpdateServices;
        IUpdateServices updateServices;
        ILateUpdateServices lateUpdateServices;
        IPlayerSound sound;
        IPlayerAnimation anim;
        IPlayerCamera cam;
        IPlayerCollision coll;
        ICrouchHandler crouch;
        ICameraHandler camHandler;
        IVelocityHandler velocity;
        IRunnningHandler run;
        ILandingHandler land;
        IJumpHandler jump;
        IDirectionHandler direction;

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out fixedUpdateServices))
                fixedUpdateServices.Register(this);
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
            if (ServiceLocator.Global.TryGet(out lateUpdateServices))
                lateUpdateServices.Register(this);
        }

        void OnDisable()
        {
            fixedUpdateServices?.Unregister(this);
            updateServices?.Unregister(this);
            lateUpdateServices?.Unregister(this);
        }

        protected override void Start()
        {
            base.Start();
            GetComponents();
            SetupStateMachine();
        }

        void GetComponents()
        {
            sound = GetComponentInChildren<IPlayerSound>();
            anim = GetComponentInChildren<IPlayerAnimation>();
            cam = GetComponentInChildren<IPlayerCamera>();
            coll = GetComponentInChildren<IPlayerCollision>();
            crouch = GetComponentInChildren<ICrouchHandler>();
            camHandler = GetComponentInChildren<ICameraHandler>();
            velocity = GetComponentInChildren<IVelocityHandler>();
            run = GetComponentInChildren<IRunnningHandler>();
            land = GetComponentInChildren<ILandingHandler>();
            jump = GetComponentInChildren<IJumpHandler>();
            direction = GetComponentInChildren<IDirectionHandler>();
        }

        void SetupStateMachine()
        {
            var groundedState = new PlayerGroundedState(
                collisionData,
                sound,
                anim,
                cam,
                coll,
                crouch,
                camHandler,
                velocity,
                run,
                land,
                jump,
                direction
            );
            var airborneState = new PlayerAirborneState(
                collisionData,
                sound,
                anim,
                cam,
                coll,
                camHandler,
                velocity,
                direction
            );

            At(groundedState, airborneState, !collisionData.OnGrounded);
            At(airborneState, groundedState, collisionData.OnGrounded);

            stateMachine.SetState(groundedState);
        }

        public void ProcessFixedUpdate(float deltaTime) => stateMachine.FixedUpdate(deltaTime);

        public void ProcessUpdate(float deltaTime) => stateMachine.Update(deltaTime);

        public void ProcessLateUpdate(float deltaTime) => stateMachine.LateUpdate(deltaTime);
    }
}
