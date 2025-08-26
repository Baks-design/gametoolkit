using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class Or : IPredicate
    {
        [SerializeField]
        List<IPredicate> rules = new();

        public bool Evaluate()
        {
            foreach (var rule in rules)
                if (rule.Evaluate())
                    return true;
            return false;
        }
    }
}
