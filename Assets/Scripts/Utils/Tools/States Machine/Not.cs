using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class Not : IPredicate
    {
        [SerializeField, TextArea]
        IPredicate rule;

        public bool Evaluate() => !rule.Evaluate();
    }
}
