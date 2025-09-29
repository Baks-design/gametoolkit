using Alchemy.Inspector;
using GameToolkit.Runtime.Game.Systems.Sound;
using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerSoundController : MonoBehaviour, IUpdatable
    {
        [SerializeField, Required]
        CharacterController controller;

        [SerializeField]
        PlayerSoundConfig soundConfig;

        [SerializeField, InlineEditor]
        SoundLibraryObject soundLibrary;

        [SerializeField, ReadOnly]
        PlayerMovementData movementData;

        [SerializeField, ReadOnly]
        PlayerCollisionData collisionData;

        IUpdateServices updateServices;
        ISoundServices soundServices;
        SoundBuilder soundBuilder;
        float footstepTimer;
        float swimmingTimer;
        float climbingTimer;
        bool wasGrounded;
        bool wasSwimming;
        bool wasClimbing;
        float fallStartHeight;
        bool isFalling;

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);

            if (ServiceLocator.Global.TryGet(out soundServices))
                soundBuilder = soundServices.CreateSoundBuilder();
        }

        void OnDisable() => updateServices?.Unregister(this);

        public void ProcessUpdate(float deltaTime)
        {
            UpdateFootsteps(deltaTime);
            UpdateLanding();
            UpdateSwimming(deltaTime);
            UpdateClimbing(deltaTime);
        }

        void UpdateFootsteps(float deltaTime)
        {
            if (
                !collisionData.OnGrounded
                || movementData.IsSwimming
                || movementData.IsClimbing
                || !movementData.IsMoving
            )
            {
                footstepTimer = 0f;
                return;
            }

            footstepTimer += deltaTime;

            // Determine footstep interval based on movement speed
            var currentInterval = movementData.IsRunning
                ? soundConfig.FootstepIntervalRun
                : soundConfig.FootstepIntervalWalk;

            if (footstepTimer >= currentInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }

        void PlayFootstepSound() =>
            soundBuilder
                .WithRandomPitch(0.9f, 1.1f)
                .WithSetVolume(soundConfig.FootstepVolume)
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.FootstepClip);

        void UpdateLanding()
        {
            var isGrounded = collisionData.OnGrounded;

            // Detect landing (was falling, now grounded)
            if (!wasGrounded && isGrounded && isFalling)
            {
                PlayLandingSound();
                isFalling = false;
            }

            // Detect start of fall
            if (wasGrounded && !isGrounded && !movementData.IsSwimming)
            {
                isFalling = true;
                fallStartHeight = transform.position.y;
            }

            wasGrounded = isGrounded;
        }

        void PlayLandingSound()
        {
            var fallDistance = fallStartHeight - controller.transform.position.y;
            var impactVolume = Mathf.Clamp(fallDistance / 5f, 0.3f, 1f) * soundConfig.LandingVolume;
            soundBuilder
                .WithRandomPitch(0.8f, 1.2f)
                .WithSetVolume(impactVolume)
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.LandingClip);
        }

        void UpdateSwimming(float deltaTime)
        {
            var isSwimming = movementData.IsSwimming;

            if (isSwimming)
            {
                if (movementData.IsMoving)
                {
                    swimmingTimer += deltaTime;
                    if (swimmingTimer >= soundConfig.SwimmingInterval)
                    {
                        PlaySwimmingSound();
                        swimmingTimer = 0f;
                    }
                }
                else
                    swimmingTimer = 0f;

                // Play entry sound when starting to swim
                if (!wasSwimming)
                    PlaySwimmingSound();
            }
            else
                swimmingTimer = 0f;

            wasSwimming = isSwimming;
        }

        void PlaySwimmingSound() =>
            soundBuilder
                .WithRandomPitch(0.8f, 1.2f)
                .WithSetVolume(soundConfig.SwimmingVolume)
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.SwimmingClip);

        void UpdateClimbing(float deltaTime)
        {
            var isClimbing = movementData.IsClimbing;

            if (isClimbing)
            {
                if (movementData.IsMoving)
                {
                    climbingTimer += deltaTime;
                    if (climbingTimer >= soundConfig.ClimbingInterval)
                    {
                        PlayClimbingSound();
                        climbingTimer = 0f;
                    }
                }
                else
                    climbingTimer = 0f;

                // Play start climbing sound
                if (!wasClimbing)
                    PlayClimbingSound();
            }
            else
                climbingTimer = 0f;

            wasClimbing = isClimbing;
        }

        void PlayClimbingSound() =>
            soundBuilder
                .WithRandomPitch(0.9f, 1.1f)
                .WithSetVolume(soundConfig.ClimbingVolume)
                .WithPosition(transform.position)
                .Play(soundLibrary.ClimbingClip);

        public void PlayJumpSound() =>
            soundBuilder
                .WithRandomPitch()
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.JumpingClip);

        public void PlayDamageSound() =>
            soundBuilder
                .WithRandomPitch()
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.DamagingClip);
    }
}
