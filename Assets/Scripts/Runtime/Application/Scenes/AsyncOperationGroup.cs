using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

namespace GameToolkit.Runtime.Application.Scenes
{
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress =>
            Operations.Count == 0 ? 0 : Operations.AsValueEnumerable().Average(o => o.progress);
        public bool IsDone => Operations.AsValueEnumerable().All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity) =>
            Operations = new List<AsyncOperation>(initialCapacity);
    }
}
