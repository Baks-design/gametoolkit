namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
