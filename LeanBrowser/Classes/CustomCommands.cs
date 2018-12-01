using System.Windows.Input;

namespace LeanBrowser
{
    public static class CustomCommands
    {
        // New tab
        public static readonly RoutedUICommand New = new RoutedUICommand
        (
            "New tab",
            "New tab",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.T, ModifierKeys.Control)
            }
        );

        // Exit
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );

        // New incognito window
        public static readonly RoutedUICommand NewIncognito = new RoutedUICommand
        (
            "New incognito window",
            "New incognito window",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );

        // Close tab
        public static readonly RoutedUICommand CloseTab = new RoutedUICommand
        (
            "Close current tab",
            "Close current tab",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.W, ModifierKeys.Control)
            }
        );

        // Toggle fullscreen
        public static readonly RoutedUICommand ToggleFullscreen = new RoutedUICommand
        (
            "Enable/disable full-screen mode",
            "Enable/disable full-screen mode",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F11)
            }
        );

        // Exit fullscreen
        public static readonly RoutedUICommand ExitFullscreen = new RoutedUICommand
        (
            "Exit full-screen mode",
            "Exit full-screen mode",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Escape)
            }
        );

        // Zoom in
        public static readonly RoutedUICommand ZoomIn = new RoutedUICommand
        (
            "Zoom in",
            "Zoom in",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.OemPlus, ModifierKeys.Control),
                new KeyGesture(Key.Add, ModifierKeys.Control)
            }
        );

        // Zoom out
        public static readonly RoutedUICommand ZoomOut = new RoutedUICommand
        (
            "Zoom out",
            "Zoom out",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.OemMinus, ModifierKeys.Control),
                new KeyGesture(Key.Subtract, ModifierKeys.Control)
            }
        );

        // Reset zoom
        public static readonly RoutedUICommand ZoomReset = new RoutedUICommand
        (
            "Reset zoom to default",
            "Reset zoom to default",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D0, ModifierKeys.Control),
                new KeyGesture(Key.NumPad0, ModifierKeys.Control)
            }
        );

        // Navigate tabs 
        public static readonly RoutedUICommand NavigateTabs = new RoutedUICommand
        (
            "Navigate through open tabs",
            "Navigate through open tabs",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Tab, ModifierKeys.Control)
            }
        );

        // Reload
        public static readonly RoutedUICommand Reload = new RoutedUICommand
        (
            "Reload",
            "Reload",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.R, ModifierKeys.Control)
            }
        );

        // Go Back
        public static readonly RoutedUICommand Back = new RoutedUICommand
        (
            "Go back",
            "Go back",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Left, ModifierKeys.Alt)
            }
        );

        // Go Forward
        public static readonly RoutedUICommand Forward = new RoutedUICommand
        (
            "Go forward",
            "Go forward",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Right, ModifierKeys.Alt)
            }
        );

        // Show favorites
        public static readonly RoutedUICommand Favorites = new RoutedUICommand
        (
            "Show favorites",
            "Show favorites",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.I, ModifierKeys.Control)
            }
        );

        // Show favorites
        public static readonly RoutedUICommand History = new RoutedUICommand
        (
            "Show history",
            "Show history",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.H, ModifierKeys.Control)
            }
        );

        // Show favorites
        public static readonly RoutedUICommand Downloads = new RoutedUICommand
        (
            "Show downloads",
            "Show downloads",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.J, ModifierKeys.Control)
            }
        );

        // View page source
        public static readonly RoutedUICommand ViewSource = new RoutedUICommand
        (
            "View page source",
            "View page source",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.U, ModifierKeys.Control)
            }
        );

        // Inspect element
        public static readonly RoutedUICommand Inspect = new RoutedUICommand
        (
            "Inspect element",
            "Inspect element",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.I, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );





























    }
}
