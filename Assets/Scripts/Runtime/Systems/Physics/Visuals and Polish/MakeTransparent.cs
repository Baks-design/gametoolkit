using UnityEngine;

namespace GameToolkit.Runtime.Systems.Physics
{
    /// <summary>
    /// Put this on objects you would like to turn transparent
    /// if they're blocking the view from the camera to the player.
    /// Note that the transparentMat needs to be transparent by default.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MakeTransparent : MonoBehaviour
    {
        [SerializeField]
        MeshRenderer myRenderer;

        [SerializeField]
        Material opaqueMat;

        [SerializeField]
        Material transparentMat;

        bool myTransparent = false;

        void Start()
        {
            if (myRenderer == null)
                TryGetComponent(out myRenderer);
        }

        public void MakeAlpha()
        {
            if (myTransparent)
                return;
            //Before, I just had the object start out as transparent and
            // then I'd change the color here to something more transparent
            // I am concerned about performance, however,
            // and the decal shadows don't work on Transparent-style materials
            myRenderer.material = transparentMat;
            myTransparent = true;
        }

        public void MakeOpaque()
        {
            if (!myTransparent)
                return;
            myRenderer.material = opaqueMat;
            myTransparent = false;
        }
    }
}
