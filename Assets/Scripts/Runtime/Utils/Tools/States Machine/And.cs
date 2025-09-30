using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class And : IPredicate
    {
        [SerializeField]
        List<IPredicate> rules = new();

        public bool Evaluate() => rules.AsValueEnumerable().All(r => r.Evaluate());
    }
}
