using System;
using Alchemy.Inspector;
using GameToolkit.Runtime.Game.Systems.Sound;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public class PlayerSound : MonoBehaviour, IPlayerSound
    {
        [SerializeField, Required]
        CharacterController controller;

        [SerializeField]
        PlayerSoundConfig soundConfig;

        [SerializeField, InlineEditor]
        SoundLibraryObject soundLibrary;

        [SerializeField, HideInInspector]
        PlayerMovementData movementData;

        [SerializeField, HideInInspector]
        PlayerCollisionData collisionData;

        ISoundServices soundServices;
        SoundBuilder soundBuilder;
        float footstepTimer;
        float swimmingTimer;
        float climbingTimer;
        bool wasSwimming;
        bool wasClimbing;
        float fallStartHeight;

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out soundServices))
                soundBuilder = soundServices.CreateSoundBuilder();
        }

        void Start() => fallStartHeight = controller.transform.position.y;

        public void UpdateFootsteps(float deltaTime)
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

        public void UpdateLanding()
        {
            var fallDistance = fallStartHeight - controller.transform.position.y;
            var impactVolume = Mathf.Clamp(fallDistance / 5f, 0.3f, 1f) * soundConfig.LandingVolume;
            PlayLandingSound(impactVolume);
        }

        void PlayLandingSound(float impactVolume) =>
            soundBuilder
                .WithRandomPitch(0.8f, 1.2f)
                .WithSetVolume(impactVolume)
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.LandingClip);

        public void UpdateSwimming(float deltaTime)
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

        public void UpdateClimbing(float deltaTime)
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

        public void UpdateJumping()
        {
            if (!movementData.IsJumping)
                return;

            PlayJumpSound();
        }

        void PlayJumpSound() =>
            soundBuilder
                .WithRandomPitch()
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.JumpingClip);

        public void UpdateDamaging() => PlayDamageSound();

        void PlayDamageSound() =>
            soundBuilder
                .WithRandomPitch()
                .WithPosition(controller.transform.position)
                .Play(soundLibrary.DamagingClip);
    }
}
