using Alchemy.Inspector;
using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField, AssetsOnly, Required]
        GameObject pauseScreen;

        void OnEnable()
        {
            InputManager.inputActions.Player.OpenPauseScreen.performed += Show;
            InputManager.inputActions.UI.ClosePauseScreen.performed += Hide;
        }

        public void Show(CallbackContext context)
        {
            EventBus<PauseScreenEvent>.Raise(new PauseScreenEvent { HasOpened = true });
            pauseScreen.SetActive(true);
        }

        public void Hide(CallbackContext context)
        {
            EventBus<PauseScreenEvent>.Raise(new PauseScreenEvent { HasOpened = false });
            pauseScreen.SetActive(false);
        }

        void OnDisable()
        {
            InputManager.inputActions.Player.OpenPauseScreen.performed -= Show;
            InputManager.inputActions.UI.ClosePauseScreen.performed -= Hide;
        }
    }
}
