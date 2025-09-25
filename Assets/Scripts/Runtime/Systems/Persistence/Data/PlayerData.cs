using System;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Persistence
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        public Vector3 position;
        public Quaternion rotation;

        [field: NonSerialized]
        public SerializableGuid Id { get; set; }
    }
}
