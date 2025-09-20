using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Persistence
{
    public class PlayerDataHandler : MonoBehaviour, IUpdatable, IBind<PlayerData>
    {
        [SerializeField]
        PlayerData data;

        [field: SerializeField]
        public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            this.data.Id = Id;
            transform.SetPositionAndRotation(data.position, data.rotation);
        }

        void OnEnable() => UpdateManager.Register(this);

        void OnDisable() => UpdateManager.Unregister(this);

        public void ProcessUpdate(float deltaTime)
        {
            data.position = transform.position;
            data.rotation = transform.rotation;
        }
    }
}
