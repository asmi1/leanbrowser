using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeanBrowser.Controls
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public MainWindow mainWindow;
        public TabView tabView;
        public double zoomLevel;

        public Menu()
        {
            mainWindow = this.FindParent<MainWindow>();
            tabView = null;
            zoomLevel = 0;

            InitializeComponent();
            SetupMenu();
        }

        private void SetupMenu()
        {

            // Tabs & windows
            newTab.SetIcon("tabDrawingImage");
            newTab.SetLabel("New tab");
            newTab.SetTip("Ctrl+T");
            newWindow.SetIcon("websiteDrawingImage");
            newWindow.SetLabel("New window");
            newWindow.SetTip("Ctrl+N");
            newPrivateWindow.SetIcon("websiteDrawingImage");
            newPrivateWindow.SetLabel("New incognito window");
            newPrivateWindow.SetTip("Ctrl+Shift+N");

            // Zoom
            fullScreen.IsEnabled = true;

            // Data
            favorites.SetIcon("favoriteDrawingImage");
            favorites.SetLabel("Favorites");
            favorites.SetTip("Ctrl+I");
            history.SetIcon("historyDrawingImage");
            history.SetLabel("History");
            history.SetTip("Ctrl+H");
            downloads.SetIcon("downloadDrawingImage");
            downloads.SetLabel("Downloads");
            downloads.SetTip("Ctrl+J");

            // Tools
            find.SetIcon("searchDrawingImage");
            find.SetLabel("Find");
            find.SetTip("Ctrl+F");
            print.SetIcon("printDrawingImage");
            print.SetLabel("Print");
            print.SetTip("Ctrl+P");

            // Extra
            settings.SetIcon("settingsDrawingImage");
            settings.SetLabel("Settings");
            help.SetIcon("helpDrawingImage");
            help.SetLabel("About");
            exit.SetIcon("exitDrawingImage");
            exit.SetLabel("Exit");
            exit.SetTip("Alt+F4");

            // commands
            newTab.btn.Command = CustomCommands.New;
            newWindow.btn.Command = ApplicationCommands.New;
            newPrivateWindow.btn.Command = CustomCommands.NewIncognito;

            zoomIn.Command = CustomCommands.ZoomIn;
            zoomOut.Command = CustomCommands.ZoomOut;
            zoomReset.Command = CustomCommands.ZoomReset;
            fullScreen.Command = CustomCommands.ToggleFullscreen;

            favorites.btn.Command = CustomCommands.Favorites;
            history.btn.Command = CustomCommands.History;
            downloads.btn.Command = CustomCommands.Downloads;

            find.btn.Command = ApplicationCommands.Find;
            print.btn.Command = ApplicationCommands.Print;
            
            exit.btn.Command = CustomCommands.Exit;
        }

        public void RefreshZoom()
        {
            if (tabView != null && tabView.WebView != null)
            {
                zoomLevel = tabView.WebView.Browser.ZoomLevel;
                zoomPercentage.Text = ZoomLevelToZoomPercentage(zoomLevel).ToString() + "%";

                zoomReset.IsEnabled = zoomLevel != 0;
                zoomOut.IsEnabled = zoomLevel > -8;
                zoomIn.IsEnabled = zoomLevel < 8;
            }
        }

        double ZoomPercentageToZoomLevel(double zoomPercentage)
        {
            return Math.Log(zoomPercentage / 100) / Math.Log(1.2);
        }

        int ZoomLevelToZoomPercentage(double zoomLevel)
        {
            return Convert.ToInt32(Math.Pow(1.2, zoomLevel) * 100);
        }
    }
}
