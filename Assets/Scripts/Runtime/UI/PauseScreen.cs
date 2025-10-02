using Alchemy.Inspector;
using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace GameToolkit.Runtime.UI
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField, AssetsOnly, Required]
        GameObject pauseScreen;
        IMovementInput movementInput;
        IUIInput uIInput;

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out movementInput))
                movementInput.OpenMenuPressed().performed += Show;

            if (ServiceLocator.Global.Get(out uIInput))
                uIInput.CloseMenuPressed().performed += Hide;
        }

        void OnDisable()
        {
            movementInput.OpenMenuPressed().performed -= Show;
            uIInput.CloseMenuPressed().performed -= Hide;
        }

        void Show(CallbackContext context)
        {
            EventBus<PauseScreenEvent>.Raise(new PauseScreenEvent { HasOpened = true });
            pauseScreen.SetActive(true);
        }

        void Hide(CallbackContext context)
        {
            EventBus<PauseScreenEvent>.Raise(new PauseScreenEvent { HasOpened = false });
            pauseScreen.SetActive(false);
        }
    }
}
