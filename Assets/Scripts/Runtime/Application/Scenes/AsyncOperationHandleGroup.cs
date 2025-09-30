using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using ZLinq;

namespace GameToolkit.Runtime.Application.Scenes
{
    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress =>
            Handles.Count == 0 ? 0 : Handles.AsValueEnumerable().Average(h => h.PercentComplete);
        public bool IsDone => Handles.Count == 0 || Handles.AsValueEnumerable().All(o => o.IsDone);

        public AsyncOperationHandleGroup(int initialCapacity) =>
            Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
    }
}
