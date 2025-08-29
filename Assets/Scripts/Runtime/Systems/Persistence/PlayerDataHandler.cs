using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Persistence
{
    public class PlayerDataHandler : CustomMonoBehaviour, IUpdatable, IBind<PlayerData>
    {
        [SerializeField]
        PlayerData data;

        [field: SerializeField]
        public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            this.data.Id = Id;

            tr.SetPositionAndRotation(data.position, data.rotation);
        }

        public void ProcessUpdate(float deltaTime)
        {
            data.position = tr.position;
            data.rotation = tr.rotation;
        }
    }
}
