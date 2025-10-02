using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class HeadBobHandler
    {
        readonly HeadBobData data;
        readonly PlayerMovementData movementData;
        float xScroll;
        float yScroll;

        public HeadBobHandler(
            HeadBobData data,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.data = data;
            this.movementData = movementData;

            data.MoveBackwardsFrequencyMultiplier = movementConfig.MoveBackwardsSpeedPercent;
            data.MoveSideFrequencyMultiplier = movementConfig.MoveSideSpeedPercent;
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
            xScroll += deltaTime * data.xFrequency * frequencyMultiplier;
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
                amplitude = data.runAmplitudeMultiplier;
                frequency = data.runFrequencyMultiplier;
            }
            if (crouching)
            {
                amplitude = data.crouchAmplitudeMultiplier;
                frequency = data.crouchFrequencyMultiplier;
            }
            return (amplitude, frequency);
        }

        float CalculateDirectionMultiplier(Vector2 input)
        {
            if (input.y < -0.1f)
                return data.MoveBackwardsFrequencyMultiplier;
            if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f)
                return data.MoveSideFrequencyMultiplier;
            return 1f;
        }

        void CalculateHeadBobOffset(float amplitudeMultiplier, float additionalMultiplier)
        {
            var xValue = data.xCurve.Evaluate(xScroll);
            var yValue = data.yCurve.Evaluate(yScroll);
            movementData.FinalOffset = new Vector3(
                xValue * data.xAmplitude * amplitudeMultiplier * additionalMultiplier,
                yValue * data.yAmplitude * amplitudeMultiplier * additionalMultiplier,
                0f
            );
        }
    }
}
