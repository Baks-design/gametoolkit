using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class And : IPredicate
    {
        [SerializeField]
        List<IPredicate> rules = new();

        public bool Evaluate()
        {
            var result = true;
            foreach (var rule in rules)
                result &= rule.Evaluate();
            return result;
        }
    }
}
