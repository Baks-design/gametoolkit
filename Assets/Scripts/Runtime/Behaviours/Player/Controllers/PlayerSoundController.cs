using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerSoundController : MonoBehaviour, IUpdatable
    {
        IUpdateServices updateServices;

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        public void ProcessUpdate(float deltaTime) { }

        void OnDisable() => updateServices.Unregister(this);
    }
}
