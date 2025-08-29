using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    ///  Attach this to stuff that you want to be affected by gravity."
    ///  Right now, it accelerates the object in the direction of the gravitational vector
    ///  but you can edit this to suit your needs.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CustomGravityAffected : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rb;
        List<GravPackageStruct> newGravs;

        public Vector3 FallingDirection { get; private set; } = Vector3.down;

        void Start()
        {
            if (rb == null)
                TryGetComponent(out rb);
            newGravs = new List<GravPackageStruct>();
        }

        void Update()
        {
            if (newGravs.Count > 0)
            {
                //I could probably pick a better number -
                // this just guarantees that *any* gravity field can override it and control the object
                var priorityTracker = -100;
                var newerGrav = Vector3.zero;
                for (var i = 0; i < newGravs.Count; i++)
                {
                    if (newGravs[i].gravPriority > priorityTracker)
                    {
                        //If you find a gravitational field with greater priority, use it instead
                        newerGrav = newGravs[i].gravVect;
                        priorityTracker = newGravs[i].gravPriority;
                    }
                }
                FallingDirection = newerGrav;
            }
            newGravs.Clear();
        }

        //Accelerate the object in the direction of gravity every physics update
        //You can replace this with whatever you want -
        //as long as it references fallingDirection in some capacity, it should look fine
        void FixedUpdate() => rb.AddForce(FallingDirection, ForceMode.Acceleration);

        //Add the gravitational vector to the list
        //This ends up updating ~every frame because stuff like
        // sphere fields and spline fields change their vectors based on position
        public void AddGravStruct(GravPackageStruct gravStruct) => newGravs.Add(gravStruct);
    }
}
