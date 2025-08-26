using System;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    /// <summary>
    /// Represents a predicate that encapsulates an action and evaluates to true once the action has been invoked.
    /// </summary>
    public class ActionPredicate : IPredicate
    {
        public bool flag;

        public ActionPredicate(ref Action eventReaction) =>
            eventReaction += () =>
            {
                flag = true;
            };

        public bool Evaluate()
        {
            var result = flag;
            flag = false;
            return result;
        }
    }
}
