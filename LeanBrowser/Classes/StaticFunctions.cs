using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LeanBrowser
{
    class StaticFunctions
    {
        public static MenuItem BuildContextMenuItem(string item, bool isEnabled, Visibility IsVisible, RoutedEventHandler clickHandler, string igt = null)
        {
            MenuItem result = new MenuItem();
            result.Header = item;
            result.Visibility = Visibility.Collapsed;
            result.Visibility = IsVisible;
            result.IsEnabled = isEnabled;
            result.Click += clickHandler;

            if (!string.IsNullOrEmpty(igt))
            {
                result.InputGestureText = igt;
            }

            return result;
        }

        public static void AnimateScale(double fromX, double fromY, double toX, double toY, UIElement control, double duration)
        {
            var fade = new DoubleAnimation
            {
                From = fromY,
                To = toY,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTarget(fade, control);
            Storyboard.SetTargetProperty(fade, new PropertyPath(FrameworkElement.HeightProperty));

            var fade2 = new DoubleAnimation
            {
                From = fromX,
                To = toX,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTarget(fade2, control);
            Storyboard.SetTargetProperty(fade2, new PropertyPath(FrameworkElement.WidthProperty));


            var sb = new Storyboard();
            sb.Children.Add(fade2);
            sb.Children.Add(fade);
            sb.Begin();
        }

        public static void AnimateHeight(double from, double to, UIElement control, double duration)
        {
            var fade = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTarget(fade, control);
            Storyboard.SetTargetProperty(fade, new PropertyPath(FrameworkElement.HeightProperty));

            var sb = new Storyboard();
            sb.Children.Add(fade);
            sb.Begin();
        }
        
        public static void AnimateFade(double from, double to, UIElement control, double duration)
        {
            var fade = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTarget(fade, control);
            Storyboard.SetTargetProperty(fade, new PropertyPath(UIElement.OpacityProperty));

            var sb = new Storyboard();
            sb.Children.Add(fade);
            sb.Begin();

        }

        public static void AnimateFade(double from, double to, UIElement control, double duration, Action method)
        {
            var fade = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(duration)
            };
            Storyboard.SetTarget(fade, control);
            Storyboard.SetTargetProperty(fade, new PropertyPath(UIElement.OpacityProperty));

            var sb = new Storyboard();
            sb.Children.Add(fade);
            sb.Completed +=
                (o, e) =>
                {
                    method();
                };
            sb.Begin();
        }
    }
}
