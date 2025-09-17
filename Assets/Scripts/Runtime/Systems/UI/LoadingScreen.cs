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
            loadingCamera.gameObject.SetActive(true);
        }

        public void SetBarPercent(float percent)
        {
            loadingBar.fillAmount = 0f;

            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - percent);
            var dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(
                currentFillAmount,
                percent,
                percent * dynamicFillSpeed
            );
        }

        public void Hide()
        {
            loadingCanvas.gameObject.SetActive(false);
            loadingCamera.gameObject.SetActive(false);
        }
    }
}
