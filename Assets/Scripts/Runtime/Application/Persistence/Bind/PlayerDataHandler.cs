using Alchemy.Inspector;
using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Application.Persistence
{
    public class PlayerDataHandler : MonoBehaviour, IUpdatable, IBind<PlayerData>
    {
        [SerializeField, Required]
        CharacterController controller;

        [SerializeField, ReadOnly]
        PlayerData data;
        IUpdateServices updateServices;

        [field: SerializeField, ReadOnly]
        public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            this.data.Id = Id;

            controller.transform.SetPositionAndRotation(data.position, data.rotation);
        }

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        public void ProcessUpdate(float deltaTime)
        {
            data.position = controller.transform.position;
            data.rotation = controller.transform.rotation;
        }

        void OnDisable() => updateServices.Unregister(this);
    }
}
