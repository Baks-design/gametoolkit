using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Input
{
    public class InputManager : MonoBehaviour, IInputServices
    {
        InputSystem_Actions inputActions = new();

        public PlayerInputMap PlayerInputMap { get; set; }
        public UIInputMap UIInputMap { get; set; }

        void Awake()
        {
            Setup();
            SetupCallbacks();
            EnablePlayerMap();
        }

        void Setup()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IInputServices>(this);
        }

        void SetupCallbacks()
        {
            PlayerInputMap = new PlayerInputMap(inputActions);
            UIInputMap = new UIInputMap(inputActions);
        }

        public void EnablePlayerMap()
        {
            inputActions.Player.Enable();
            inputActions.UI.Disable();
        }

        public void EnableUIMap()
        {
            inputActions.UI.Enable();
            inputActions.Player.Disable();
        }
    }
}
