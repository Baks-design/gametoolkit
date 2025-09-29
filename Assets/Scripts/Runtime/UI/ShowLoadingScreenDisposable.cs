using System;

namespace GameToolkit.Runtime.UI
{
    public class ShowLoadingScreenDisposable : IDisposable
    {
        readonly LoadingScreen loadingScreen;

        public ShowLoadingScreenDisposable(LoadingScreen loadingScreen)
        {
            this.loadingScreen = loadingScreen;
            loadingScreen.Show();
        }

        public void SetLoadingBarPercent(float percent) => loadingScreen.SetBarPercent(percent);

        public void Dispose() => loadingScreen.Hide();
    }
}
