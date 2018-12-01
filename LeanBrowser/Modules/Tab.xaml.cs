using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DotNetBrowser;
using LeanBrowser.Controls;

namespace LeanBrowser
{
    public partial class Tab : UserControl
    {
        public UserControl form;
        public MainWindow mainWindow;
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                if (value)
                {
                    tab.Style = selectedTabStyle;
                    tabOverlay.Style = selectedTabOverlayStyle;

                }
                else
                {
                    tab.Style = tabStyle;
                    tabOverlay.Style = tabOverlayStyle;
                }
            }
        }

        // Styles
        private readonly Style tabStyle = (Style)Application.Current.Resources["tabStyle"];
        private readonly Style selectedTabStyle = (Style)Application.Current.Resources["selectedTabStyle"];
        private readonly Style tabOverlayStyle = (Style)Application.Current.Resources["tabOverlayStyle"];
        private readonly Style selectedTabOverlayStyle = (Style)Application.Current.Resources["selectedTabOverlayStyle"];

        public Tab(string title, MainWindow mw, UserControl uc)
        {
            InitializeComponent();
            form = uc;
            form.HorizontalAlignment = HorizontalAlignment.Stretch;
            form.VerticalAlignment = VerticalAlignment.Stretch;
            label_TabTitle.Text = title;
            Loaded += Tab_Loaded;
            mainWindow = mw;
        }

        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            TabBar tb = this.FindParent<TabBar>();
            mainWindow.container.Children.Add(form);
            tb.SelectTab(this);
        }
        
        public void SetIcon(BitmapImage icon)
        {
            FavIcon.Source = icon;
        }

        public void SetTitle(string title)
        {
            label_TabTitle.Text = title;
        }

        public void StartLoading()
        {
            FavSpinner.Visibility = Visibility.Visible;
            FavIcon.Visibility = Visibility.Hidden;
        }

        public void StopLoading()
        {
            FavSpinner.Visibility = Visibility.Hidden;
            FavIcon.Visibility = Visibility.Visible;
        }

        public void UpdateAudioMute(Browser browser)
        {
            Dispatcher.BeginInvoke((Action)(() => {
                // If audio is muted then unmute, else mute
                AudioMute.Source = browser.AudioMuted? (ImageSource)Application.Current.Resources["audioOffDrawingImage"] : (ImageSource)Application.Current.Resources["audioOnDrawingImage"];
            }));
        }

        public void SetAudioMuteVisibility(Visibility visibility)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                AudioMute.Visibility = visibility;
            }));
        }

        private void Me_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TabBar tb = this.FindParent<TabBar>();
            tb.SelectTab(this);
        }

        private void CloseTab_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabBar tb = this.FindParent<TabBar>();
            tb.RemoveTab(this);
        }

        private void AudioMute_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var tv = form as TabView;
            tv.WebView.Browser.AudioMuted = !tv.WebView.Browser.AudioMuted;
            UpdateAudioMute(tv.WebView.Browser);
        }
    }
}