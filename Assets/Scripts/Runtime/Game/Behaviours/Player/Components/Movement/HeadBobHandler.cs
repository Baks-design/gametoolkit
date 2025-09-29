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

            float amplitudeMultiplier;
            float frequencyMultiplier;
            float additionalMultiplier; // when moving backwards or to sides

            amplitudeMultiplier = running ? data.runAmplitudeMultiplier : 1f;
            amplitudeMultiplier = crouching ? data.crouchAmplitudeMultiplier : amplitudeMultiplier;

            frequencyMultiplier = running ? data.runFrequencyMultiplier : 1f;
            frequencyMultiplier = crouching ? data.crouchFrequencyMultiplier : frequencyMultiplier;

            additionalMultiplier = input.y == -1f ? data.MoveBackwardsFrequencyMultiplier : 1f;
            additionalMultiplier =
                input.x != 0f && input.y == 0f
                    ? data.MoveSideFrequencyMultiplier
                    : additionalMultiplier;

            // you can also multiply this by additionalMultiplier but it looks unnatural a bit;
            xScroll += deltaTime * data.xFrequency * frequencyMultiplier;

            float xValue;
            float yValue;

            xValue = data.xCurve.Evaluate(xScroll);
            yValue = data.yCurve.Evaluate(yScroll);

            movementData.FinalOffset.x =
                xValue * data.xAmplitude * amplitudeMultiplier * additionalMultiplier;
            movementData.FinalOffset.y =
                yValue * data.yAmplitude * amplitudeMultiplier * additionalMultiplier;
        }

        public void ResetHeadBob()
        {
            movementData.Resetted = true;
            xScroll = yScroll = 0f;
            movementData.FinalOffset = Vector3.zero;
        }
    }
}
