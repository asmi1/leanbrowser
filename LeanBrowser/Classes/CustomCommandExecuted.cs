using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LeanBrowser
{
    public static class CustomCommandExecuted
    {

        // Open new tab command
        public static void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            var commandParams = (OpenTabCommandParameters)e.Parameter;

            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            mainWindow.TabBar.AddTab(commandParams, mainWindow);
        }

        // Close current tab command
        public static void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                Tab tab = mainWindow.TabBar.getTabFromForm(tv);
                mainWindow.TabBar.RemoveTab(tab);
            }
        }

        // New window command
        public static async void OpenNewWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            var commandParams = (OpenWindowCommandParameters)e.Parameter;

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            try
            {
                await Task.Factory.StartNew(() =>
                NewWindow(commandParams, mainWindow), CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening new window: " + ex.Message + " " + ex.Data);
            }
        }

        // New incognito command
        public static async void OpenNewIncognitoWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            var commandParams = (OpenWindowCommandParameters)e.Parameter;

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            try
            {
                await Task.Factory.StartNew(() =>
                NewWindow(commandParams, mainWindow, true), CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening new incognito window: " + ex.Message + " " + ex.Data);
            }
        }

        public static void NewWindow(OpenWindowCommandParameters commandParams, MainWindow mw, bool IsIncognito = false)
        {
            string[] args;
            args = commandParams != null ? commandParams.Args : new string[] { };

            if (IsIncognito)
            {
                List<string> argsList = new List<string>(args);
                argsList.Add("-i");
                args = argsList.ToArray();
            }

            MainWindow newWindow = new MainWindow(args);
            newWindow.Show();
        }

        // Navigate tabs command
        public static void NavigateTabs(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            int tabIndex = mainWindow.TabBar.GetSelectedTabIndex();
            mainWindow.TabBar.SelectTab(mainWindow.TabBar.TabCollection[tabIndex]);
        }

        // Zoom in command
        public static void ZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                mainWindow.Menu.zoomLevel += 1.2; // Zoom in by 20%
                tv.WebView.Browser.ZoomLevel = mainWindow.Menu.zoomLevel;
                mainWindow.Menu.RefreshZoom();
            }
        }

        // Zoom out command
        public static void ZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                mainWindow.Menu.zoomLevel -= 1.2; // Zoom out by 20%
                tv.WebView.Browser.ZoomLevel = mainWindow.Menu.zoomLevel;
                mainWindow.Menu.RefreshZoom();
            }
        }

        // Zoom reset command
        public static void ZoomReset(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                mainWindow.Menu.zoomLevel = 0;
                tv.WebView.Browser.ZoomLevel = mainWindow.Menu.zoomLevel;
                mainWindow.Menu.RefreshZoom();
            }
        }

        // Toggle full-screen mode command
        public static void ToggleFullscreen(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            mainWindow.IsFullScreen = !mainWindow.IsFullScreen;
        }

        // Exit full-screen mode command
        public static void ExitFullscreen(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            if (mainWindow.IsFullScreen)
            {
                mainWindow.IsFullScreen = false;
            }
        }

        // Reload command
        public static void Reload(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                tv.WebView.Browser.Reload();
            }
        }

        // Go back command
        public static void GoBack(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                tv.WebView.Browser.GoBack();
            }
        }

        // Go forward command
        public static void GoForward(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                tv.WebView.Browser.GoForward();
            }
        }

        // Exit window command
        public static void ExitWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            mainWindow.Close();
        }

        // Print command
        public static void PrintCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow))   // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null)
            {
                tv.WebView.Browser.Print();
            }
        }

        // Find text command
        public static void FindCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Find invoked");
        }

        // Show favorites command
        public static void ShowFavorites(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Favorites invoked");
        }

        // Show history command
        public static void ShowHistory(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("History invoked");
        }

        // Show downloads command
        public static void ShowDownloads(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Downloads invoked");
        }
        
        // View page source
        public static void ViewSource(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is MainWindow mainWindow)) // Return if not a main window
                return;

            TabView tv = mainWindow.getActiveTabView();
            if (tv != null && !tv.WebView.Browser.URL.StartsWith("view-source:"))
            {
                string sourceURL = "view-source:" + tv.WebView.Browser.URL;
                CustomCommands.New.Execute(new OpenTabCommandParameters(sourceURL, sourceURL), mainWindow);
            }
        }

        // Inspect element
        public static void Inspect(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Inspect invoked");
        }
    }
}
