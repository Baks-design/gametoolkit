using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class Or : IPredicate
    {
        [SerializeField]
        List<IPredicate> rules = new();

        public bool Evaluate() => rules.AsValueEnumerable().Any(r => r.Evaluate());
    }
}
