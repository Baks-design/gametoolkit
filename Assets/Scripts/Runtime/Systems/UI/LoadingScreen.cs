using Alchemy.Inspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField, AssetsOnly, Required]
        Image loadingBar;

        [SerializeField, AssetsOnly, Required]
        Canvas loadingCanvas;

        [SerializeField, AssetsOnly, Required]
        CinemachineCamera loadingCamera;

        public void Show()
        {
            loadingCanvas.gameObject.SetActive(true);
            loadingCamera.Priority.Value = 99;
        }

        public void SetBarPercent(float percent) => loadingBar.fillAmount = percent;

        public void Hide()
        {
            loadingCanvas.gameObject.SetActive(false);
            loadingCamera.Priority.Value = 0;
        }
    }
}
