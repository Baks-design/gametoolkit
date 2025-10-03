using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class HeadBobHandler
    {
        readonly HeadBobConfig config;
        readonly PlayerMovementData movementData;
        float xScroll;
        float yScroll;

        public HeadBobHandler(
            HeadBobConfig config,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.config = config;
            this.movementData = movementData;

            config.MoveBackwardsFrequencyMultiplier = movementConfig.MoveBackwardsSpeedPercent;
            config.MoveSideFrequencyMultiplier = movementConfig.MoveSideSpeedPercent;
            movementData.Resetted = false;
            movementData.FinalOffset = Vector3.zero;
            xScroll = yScroll = 0f;
        }

        public void ScrollHeadBob(bool running, bool crouching, Vector2 input, float deltaTime)
        {
            movementData.Resetted = false;

            (var amplitudeMultiplier, var frequencyMultiplier) = CalculateMovementMultipliers(
                running,
                crouching
            );
            var additionalMultiplier = CalculateDirectionMultiplier(input);
            xScroll += deltaTime * config.xFrequency * frequencyMultiplier;
            CalculateHeadBobOffset(amplitudeMultiplier, additionalMultiplier);
        }

        public void ResetHeadBob()
        {
            movementData.Resetted = true;
            xScroll = yScroll = 0f;
            movementData.FinalOffset = Vector3.zero;
        }

        (float amplitude, float frequency) CalculateMovementMultipliers(
            bool running,
            bool crouching
        )
        {
            var amplitude = 1f;
            var frequency = 1f;
            if (running)
            {
                amplitude = config.runAmplitudeMultiplier;
                frequency = config.runFrequencyMultiplier;
            }
            if (crouching)
            {
                amplitude = config.crouchAmplitudeMultiplier;
                frequency = config.crouchFrequencyMultiplier;
            }
            return (amplitude, frequency);
        }

        float CalculateDirectionMultiplier(Vector2 input)
        {
            if (input.y < -0.1f)
                return config.MoveBackwardsFrequencyMultiplier;
            if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f)
                return config.MoveSideFrequencyMultiplier;
            return 1f;
        }

        void CalculateHeadBobOffset(float amplitudeMultiplier, float additionalMultiplier)
        {
            var xValue = config.xCurve.Evaluate(xScroll);
            var yValue = config.yCurve.Evaluate(yScroll);
            movementData.FinalOffset = new Vector3(
                xValue * config.xAmplitude * amplitudeMultiplier * additionalMultiplier,
                yValue * config.yAmplitude * amplitudeMultiplier * additionalMultiplier,
                0f
            );
        }
    }
}
