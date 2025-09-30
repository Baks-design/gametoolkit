using Alchemy.Inspector;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class Not : IPredicate
    {
        [SerializeField, LabelWidth(80f)]
        IPredicate rule;

        public bool Evaluate() => !rule.Evaluate();
    }
}
