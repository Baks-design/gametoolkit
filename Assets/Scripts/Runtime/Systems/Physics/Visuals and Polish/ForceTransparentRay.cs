using System.Collections.Generic;
using UnityEngine;
using UPhysics = UnityEngine.Physics;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    /// Put this on the camera - gravity can lead to walls unexpectedly blocking the view of the camera.
    /// To prevent jarring movement, you can just turn it transparent.
    /// Note that the objects need the MakeTransparent script attached if you want them to turn transparent.
    /// </summary>
    public class ForceTransparentRay : MonoBehaviour
    {
        [SerializeField]
        Player player;
        readonly List<MakeTransparent> currentlyTrans = new();
        readonly List<MakeTransparent> previouslyTrans = new();

        void Update()
        {
            Cast();
            ForceTrans();
            UnTrans();
        }

        void Cast()
        {
            //Save the stuff from the previous frame onto the previouslyTrans list and clear the current list
            previouslyTrans.Clear();
            for (var i = 0; i < currentlyTrans.Count; i++)
                previouslyTrans.Add(currentlyTrans[i]);
            currentlyTrans.Clear();

            //Cast a ray that goes between the camera and player and save the stuff it hits
            var stuffHit = UPhysics.RaycastAll(
                transform.position,
                player.camToPlayer * -1f,
                player.camToPlayer.magnitude
            );
            if (stuffHit != null)
            {
                for (var i = 0; i < stuffHit.Length; i++)
                {
                    if (
                        stuffHit[i]
                            .collider.gameObject.TryGetComponent<MakeTransparent>(
                                out var potentialTrans
                            )
                    )
                        currentlyTrans.Add(potentialTrans);
                }
            }
        }

        //Make anything hit by the ray transparent
        public void ForceTrans()
        {
            if (currentlyTrans.Count > 0)
                for (var i = 0; i < currentlyTrans.Count; i++)
                    currentlyTrans[i].MakeAlpha();
        }

        public void UnTrans()
        {
            //If something was part of the previous frame, but not this one, turn it opaque again
            if (previouslyTrans.Count > 0)
                for (var i = 0; i < previouslyTrans.Count; i++)
                    if (currentlyTrans.Contains(previouslyTrans[i]) == false)
                        previouslyTrans[i].MakeOpaque();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, player.camToPlayer * -1f);
        }
    }
}
