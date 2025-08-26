using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManaged
{
    public abstract class CustomMonoBehaviour : MonoBehaviour, IManagedObject
    {
        IUpdateServices updateServices;

        protected virtual void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        protected virtual void OnDisable()
        {
            if (updateServices == null)
                return;
            updateServices.Unregister(this);
        }
    }
}
