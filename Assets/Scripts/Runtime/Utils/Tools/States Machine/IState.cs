namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public interface IState
    {
        void OnEnter() { }
        void FixedUpdate(float deltaTime) { }
        void Update(float deltaTime) { }
        void LateUpdate(float deltaTime) { }
        void OnExit() { }
    }
}
