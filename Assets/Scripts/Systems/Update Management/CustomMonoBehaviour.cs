using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public abstract class CustomMonoBehaviour : MonoBehaviour, IManagedObject
    {
        protected Transform tr;
        IUpdateServices updateServices;

        protected virtual void Awake() => tr = transform;

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
