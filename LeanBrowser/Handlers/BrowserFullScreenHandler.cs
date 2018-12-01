using DotNetBrowser;

namespace LeanBrowser
{
    class BrowserFullScreenHandler : FullScreenHandler
    {
        private MainWindow mainWindow;

        public BrowserFullScreenHandler(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void OnFullScreenEnter()
        {
            mainWindow.IsFullScreen = true;
        }

        public void OnFullScreenExit()
        {
            mainWindow.IsFullScreen = false;
        }
    }
}
