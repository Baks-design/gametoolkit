namespace GameToolkit.Runtime.Systems.Input
{
    public interface IInputServices
    {
        PlayerInputMap PlayerInputMap { get; set; }
        UIInputMap UIInputMap { get; set; }

        void EnablePlayerMap();
        void EnableUIMap();
    }
}
