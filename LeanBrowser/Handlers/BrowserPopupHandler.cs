using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using DotNetBrowser;
using DotNetBrowser.Events;
using DotNetBrowser.WPF;

namespace LeanBrowser
{
    public class BrowserPopupHandler : PopupHandler
    {
        MainWindow mainWindow;
        private FrameworkElement control;

        public BrowserPopupHandler(MainWindow mainWindow, FrameworkElement control)
        {
            this.mainWindow = mainWindow;
            this.control = control;
        }

        public PopupContainer HandlePopup(PopupParams popupParams)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                string[] args = new string[] { popupParams.URL.ToString() };
                // Open popups in new tab
                //CustomCommands.New.Execute(new OpenTabCommandParameters(popupParams.URL, popupParams.TargetName), mainWindow);

                // Open popups in new window
                ApplicationCommands.New.Execute(new OpenWindowCommandParameters(args), mainWindow);
            }));
            return null;
        }
    }
}