using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Persistence
{
    public class PlayerDataHandler : CustomMonoBehaviour, IBind<PlayerData>
    {
        [SerializeField]
        PlayerData data;

        [field: SerializeField]
        public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            this.data.Id = Id;
            Transform.SetPositionAndRotation(data.position, data.rotation);
        }

        public override void ProcessUpdate(float deltaTime)
        {
            data.position = Transform.position;
            data.rotation = Transform.rotation;
        }
    }
}
