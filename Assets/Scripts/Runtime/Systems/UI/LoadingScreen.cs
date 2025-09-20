using GameToolkit.Runtime.Utils.Helpers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        float fillSpeed = 0.5f;

        [SerializeField]
        Image loadingBar;

        [SerializeField]
        Canvas loadingCanvas;

        [SerializeField]
        CinemachineCamera loadingCamera;

        public void Show()
        {
            loadingCanvas.gameObject.SetActive(true);
            loadingCamera.Priority.Value = 99;
        }

        public void SetBarPercent(float percent) =>
            loadingBar.fillAmount = Mathfs.Eerp(loadingBar.fillAmount, percent, fillSpeed);

        public void Hide()
        {
            loadingCanvas.gameObject.SetActive(false);
            loadingCamera.Priority.Value = 1;
        }
    }
}
