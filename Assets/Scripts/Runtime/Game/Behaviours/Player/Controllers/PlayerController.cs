using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerController : StatefulEntity, IFixedUpdatable, IUpdatable, ILateUpdatable
    {
        [SerializeField]
        InterfaceReference<IPlayerSound> sound;

        [SerializeField]
        InterfaceReference<IPlayerAnimation> anim;

        [SerializeField]
        InterfaceReference<IPlayerCamera> cam;

        [SerializeField]
        InterfaceReference<IPlayerCollision> coll;

        [SerializeField]
        InterfaceReference<ICrouchingHandler> crouch;

        [SerializeField]
        InterfaceReference<ICameraHandler> camHandler;

        [SerializeField]
        InterfaceReference<IVelocityHandler> velocity;

        [SerializeField]
        InterfaceReference<IRunnningHandler> run;

        [SerializeField]
        InterfaceReference<ILandingHandler> land;

        [SerializeField]
        InterfaceReference<IJumpingHandler> jump;

        [SerializeField]
        InterfaceReference<IDirectionHandler> direction;

        [SerializeField, HideInInspector]
        PlayerCollisionData collisionData;

        IFixedUpdateServices fixedUpdateServices;
        IUpdateServices updateServices;
        ILateUpdateServices lateUpdateServices;

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
            SetupStateMachine();
        }

        void SetupStateMachine()
        {
            var groundedState = new PlayerGroundedState(
                collisionData,
                sound.Value,
                anim.Value,
                cam.Value,
                coll.Value,
                crouch.Value,
                camHandler.Value,
                velocity.Value,
                run.Value,
                land.Value,
                jump.Value,
                direction.Value
            );
            var airborneState = new PlayerAirborneState(
                collisionData,
                anim.Value,
                cam.Value,
                coll.Value,
                camHandler.Value,
                velocity.Value,
                direction.Value,
                jump.Value
            );

            At(groundedState, airborneState, !collisionData.OnGrounded);
            At(airborneState, groundedState, collisionData.OnGrounded);

            stateMachine.SetState(groundedState);
        }

        public void ProcessFixedUpdate(float deltaTime) => stateMachine.FixedUpdate(deltaTime);

        public void ProcessUpdate(float deltaTime, float time) =>
            stateMachine.Update(deltaTime, time);

        public void ProcessLateUpdate(float deltaTime) => stateMachine.LateUpdate(deltaTime);
    }
}
