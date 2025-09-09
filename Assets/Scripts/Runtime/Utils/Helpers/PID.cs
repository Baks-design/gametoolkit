using System;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public class PID
    {
        // The proportional, integral, and derivative gains
        readonly float pGain = 1f;
        readonly float iGain = 0f;
        readonly float dGain = 0.1f;

        // The integrated error and the last error for derivative calculation
        [NonSerialized]
        public float Integral;

        [NonSerialized]
        public float LastError;

        public PID(float p, float i, float d)
        {
            pGain = p;
            iGain = i;
            dGain = d;
        }

        public PID() { }

        // The core function. Call this every Update/FixedUpdate to get the output.
        // dt is the time step (e.g., Time.deltaTime) for frame-rate independence.
        public float Calculate(
            float target,
            float current,
            float dt,
            float outputClamp = Mathf.Infinity
        )
        {
            if (dt < Mathf.Epsilon)
                return 0f;

            var error = target - current;

            // Calculate the P and D terms first
            var p = pGain * error;
            var d = dGain * (error - LastError) / dt;
            LastError = error;

            // Calculate what the output would be without the I term
            var outputWithoutI = p + d;

            // Now calculate the I term, but clamp it based on the remaining "room" in the output
            var i = iGain * Integral;

            // Clamp the total output
            var totalOutput = outputWithoutI + i;
            totalOutput = Mathf.Clamp(totalOutput, -outputClamp, outputClamp);

            // Now, only integrate the error if it wouldn't cause windup.
            // This is a simple form of anti-windup.
            if (outputClamp != Mathf.Infinity)
            {
                // Only integrate if the output is not saturated, or if the error is in the direction that reduces saturation.
                if (
                    Mathf.Abs(totalOutput) < outputClamp
                    || Mathf.Sign(error) != Mathf.Sign(totalOutput)
                )
                    Integral += error * dt;
            }
            else
                Integral += error * dt;

            return totalOutput;
        }

        // Reset the controller's internal state. Crucial for reuse!
        public void Reset()
        {
            Integral = 0f;
            LastError = 0f;
        }
    }
}
