using System;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Persistence
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        public Vector3 position;
        public Quaternion rotation;

        [field: SerializeField]
        public SerializableGuid Id { get; set; }
    }
}
