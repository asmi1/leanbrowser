using DotNetBrowser;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace LeanBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public List<TabView> Pages;
        private bool restoreIfMove;
        public WindowState lastWinState;
        private string urlToLoad { get; set; }
        public bool isIncognito;

        private bool _IsFullScreen;
        public bool IsFullScreen
        {
            get { return _IsFullScreen; }
            set
            {
                _IsFullScreen = value;
                if (value)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MenuPopup.IsOpen = false;
                        WindowChrome.Visibility = Visibility.Collapsed;
                        ToolBar.Visibility = Visibility.Collapsed;

                        if (WindowState == WindowState.Maximized)
                        {
                            WindowState = WindowState.Normal;
                        }
                        WindowState = WindowState.Maximized;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        MenuPopup.IsOpen = false;
                        if (WindowState == WindowState.Maximized)
                        {
                            WindowState = WindowState.Normal;
                        }
                        WindowState = WindowState.Maximized;
                        WindowState = lastWinState;

                        WindowChrome.Visibility = Visibility.Visible;
                        ToolBar.Visibility = Visibility.Visible;
                    });
                }
            }
        }

        // Incognito Browser context
        public static BrowserContext iContext = 
            new BrowserContext(
                new BrowserContextParams("data") { StorageType = StorageType.MEMORY });

        public MainWindow(string[] args)
        {
            InitializeComponent();

            urlToLoad = "";
            isIncognito = false;
            IsFullScreen = false;
            restoreIfMove = false;
            lastWinState = WindowState;

            // Command bindings
            CommandBindings.Add(new CommandBinding(CustomCommands.New, CustomCommandExecuted.OpenNewTab));                      // New tab
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, CustomCommandExecuted.OpenNewWindow));              // New window
            CommandBindings.Add(new CommandBinding(CustomCommands.NewIncognito, CustomCommandExecuted.OpenNewIncognitoWindow)); // New incognito window
            CommandBindings.Add(new CommandBinding(CustomCommands.CloseTab, CustomCommandExecuted.CloseTab));                   // Close current tab
            CommandBindings.Add(new CommandBinding(CustomCommands.NavigateTabs, CustomCommandExecuted.NavigateTabs));           // Navigate tabs
            CommandBindings.Add(new CommandBinding(CustomCommands.ZoomIn, CustomCommandExecuted.ZoomIn));                       // Zoom in
            CommandBindings.Add(new CommandBinding(CustomCommands.ZoomOut, CustomCommandExecuted.ZoomOut));                     // Zoom out
            CommandBindings.Add(new CommandBinding(CustomCommands.ZoomReset, CustomCommandExecuted.ZoomReset));                 // Zoom reset
            CommandBindings.Add(new CommandBinding(CustomCommands.ToggleFullscreen, CustomCommandExecuted.ToggleFullscreen));   // Toggle fullscreen mode
            CommandBindings.Add(new CommandBinding(CustomCommands.ExitFullscreen, CustomCommandExecuted.ExitFullscreen));       // Exit fullscreen mode
            CommandBindings.Add(new CommandBinding(CustomCommands.Reload, CustomCommandExecuted.Reload));                       // Reload
            CommandBindings.Add(new CommandBinding(CustomCommands.Back, CustomCommandExecuted.GoBack));                         // Back
            CommandBindings.Add(new CommandBinding(CustomCommands.Forward, CustomCommandExecuted.GoForward));                   // Forward
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, CustomCommandExecuted.PrintCommand));             // Print
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, CustomCommandExecuted.FindCommand));               // Find
            CommandBindings.Add(new CommandBinding(CustomCommands.Favorites, CustomCommandExecuted.ShowFavorites));             // Favorites
            CommandBindings.Add(new CommandBinding(CustomCommands.History, CustomCommandExecuted.ShowHistory));                 // History
            CommandBindings.Add(new CommandBinding(CustomCommands.Downloads, CustomCommandExecuted.ShowDownloads));             // Downloads
            CommandBindings.Add(new CommandBinding(CustomCommands.Exit, CustomCommandExecuted.ExitWindow));                     // Exit Window

            CommandBindings.Add(new CommandBinding(CustomCommands.ViewSource, CustomCommandExecuted.ViewSource));               // View page source
            CommandBindings.Add(new CommandBinding(CustomCommands.Inspect, CustomCommandExecuted.Inspect));                     // Inspect element

            // Parse arguments
            if (args != null)
            {
                // Check if incognito
                if (Array.IndexOf(args, "-i") > -1 || Array.IndexOf(args, "--incognito") > -1)
                {
                    isIncognito = true;
                }

                // Check if specific URL was passed
                foreach (string element in args)
                {
                    if (Uri.IsWellFormedUriString(element, UriKind.Absolute))
                    {
                        urlToLoad = element;
                        break;
                    }
                }
            }

            Loaded += MainWindow_Loaded;
            Loaded += new RoutedEventHandler(Popup_Loaded);

            Pages = new List<TabView>();
        }

        // Main window loaded
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (WindowUtils.IsLegacyWindows)
            {
                Background = Brushes.White;
            }

            if (isIncognito == true)
            {
                TabBar.incognito.Visibility = Visibility.Visible;
            }

            // Raise command to open new tab
            CustomCommands.New.Execute(new OpenTabCommandParameters(urlToLoad, "New Tab"), this);

            WindowHandle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(WindowHandle)?.AddHook(new HwndSourceHook(HandleMessages));
        }

        // Get the current active tab
        public TabView getActiveTabView()
        {
            if (container.Children.Count > 0)
            {
                foreach (TabView tabView in container.Children)
                {
                    if (tabView.Visibility == Visibility.Visible)
                    {
                        return tabView;
                    }
                }
            }

            return null;
        }

        // Override OnClose
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //Application.Current.Shutdown();

            foreach (TabView Page in Pages)
            {
                Page.WebView.Dispose();
                Page.WebView.Browser.Dispose();
            }
        }


        public static IntPtr WindowHandle { get; private set; }

        internal void HandleParameter(string[] args)
        {
            // Raise command to open new window
            ApplicationCommands.New.Execute(new OpenWindowCommandParameters(args), this);
        }

        private IntPtr HandleMessages(IntPtr handle, int message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            var data = UnsafeNative.GetMessage(message, lParameter);

            if (data != null)
            {
                if (this == null)
                    return IntPtr.Zero;

                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal;

                UnsafeNative.SetForegroundWindow(new WindowInteropHelper
                                                (this).Handle);

                var args = data.Split(' ');
                HandleParameter(args);
                handled = true;
            }

            return IntPtr.Zero;
        }


        /// <summary>
        /// Window commands & events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_MinimizeWExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CommandBinding_MaximizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void CommandBinding_RestoreExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            InitWindowMode();
        }

        private void Window_ActivatedDeactivated(object sender, EventArgs e)
        {
            MenuPopup.IsOpen = false;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            InitWindowMode();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TabBar.TabCollection.Count > 1)
            {
                DialogBox dialog = new DialogBox
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dialog.msgTitle.Text = "Do you want to close all tabs?";
                dialog.Title = dialog.msgTitle.Text;
                dialog.ok.Content = string.Format("Close all ({0})", TabBar.TabCollection.Count);
                dialog.ShowDialog();

                if (dialog.DialogResult == false)
                {
                    e.Cancel = true;
                }
            }
        }


        private void Window_SizeChanged(object sender, EventArgs e)
        {
            InitWindowMode();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                TabBar.RefreshTabWidth();
            }));
        }

        private void DragArea_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    SwitchWindowState();
                }

                return;
            }

            else if (WindowState == WindowState.Maximized)
            {
                restoreIfMove = true;
                return;
            }

            DragMove();
        }

        private void DragArea_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            restoreIfMove = false;
        }

        private void DragArea_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (restoreIfMove)
            {
                restoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                POINT lMousePosition;
                GetCursorPos(out lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                SwitchWindowState();
                DragMove();
            }
        }

        public void InitWindowMode()
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }

            lastWinState = WindowState;
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            if (IsFullScreen)
            {
                return;
            }

            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }



        /// Provides a way to "dock" the Popup control to the Window
        ///  so that the popup "sticks" to the window while the window is dragged around.
        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(container);
            // w should not be Null now!
            if (null != w)
            {
                w.LocationChanged += delegate (object sender2, EventArgs args)
                {
                    UpdatePopupPosition(MenuPopup);
                    UpdatePopupPosition(StatusBarPopup);
                };
                // Also handle the window being resized (so the popup's position stays
                //  relative to its target element if the target element moves upon 
                //  window resize)
                w.SizeChanged += delegate (object sender3, SizeChangedEventArgs e2)
                {
                    UpdatePopupPosition(MenuPopup);
                    UpdatePopupPosition(StatusBarPopup);
                };
            }
        }

        private void UpdatePopupPosition(Popup popup)
        {
            var offset = popup.HorizontalOffset;
            popup.HorizontalOffset = offset + 1;
            popup.HorizontalOffset = offset;
        }
    }
}
