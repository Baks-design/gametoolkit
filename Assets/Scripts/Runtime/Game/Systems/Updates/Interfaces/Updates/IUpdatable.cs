namespace GameToolkit.Runtime.Game.Systems.Update
{
    public interface IUpdatable
    {
        void ProcessUpdate(float deltaTime, float time);
    }
}
