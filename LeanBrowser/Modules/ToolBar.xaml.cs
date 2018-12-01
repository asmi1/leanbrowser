using LeanBrowser.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LeanBrowser
{
    /// <summary>
    /// Interaction logic for ToolBar.xaml
    /// </summary>
    public partial class ToolBar : UserControl
    {
        public MainWindow mainWindow;

        private TabView _tabView;
        public TabView TabView
        {
            get { return _tabView; }
            set
            {
                _tabView = value;
                if (value != null)
                {
                    RefreshToolbar();
                }
                else
                {
                    InitToolbar();
                }
            }
        }

        public ToolBar()
        {
            InitializeComponent();
            Loaded += ToolBar_Loaded;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            // commands
            Back.btn.Click += Back_Click;
            Forward.btn.Click += Forward_Click;
            Refresh.btn.Click += Refresh_Click;
            Home.btn.Click += Home_Click;
            Menu.btn.Click += Menu_Click;

            mainWindow = this.FindParent<MainWindow>();
            TabView = null;
        }

        public void InitToolbar()
        {
            Back.btn.IsEnabled = false;
            Back.SetIcon("arrowLeftDrawingImageGray");
            Forward.btn.IsEnabled = false;
            Forward.SetIcon("arrowRightDrawingImageGray");
            Refresh.SetIcon("refreshDrawingImage");
            Home.SetIcon("homeDrawingImage");
            Menu.SetIcon("menuDrawingImage");

            AddressBar.ResetAddress();
        }

        public void RefreshToolbar()
        {
            if (TabView.WebView == null)
            {
                return;
            }

            if (TabView.WebView.Browser.CanGoBack())
            {
                Back.btn.IsEnabled = true;
                Back.SetIcon("arrowLeftDrawingImage");
            } else
            {
                Back.btn.IsEnabled = false;
                Back.SetIcon("arrowLeftDrawingImageGray");
            }

            if (TabView.WebView.Browser.CanGoForward())
            {
                Forward.btn.IsEnabled = true;
                Forward.SetIcon("arrowRightDrawingImage");
            }
            else
            {
                Forward.btn.IsEnabled = false;
                Forward.SetIcon("arrowRightDrawingImageGray");
            }

            if (TabView.WebView.Browser.Loading)
            {
                Refresh.SetIcon("closeDrawingImage");
                Refresh.image.Width = 12;
            } else
            {
                Refresh.SetIcon("refreshDrawingImage");
                Refresh.image.Width = 16;
                AddressBar.SetAddress(TabView.WebView.Browser.URL, TabView.certificates, TabView.certificateErrors);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (TabView != null)
            {
                TabView.WebView.Browser.GoBack();
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (TabView != null)
            {
                TabView.WebView.Browser.GoForward();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (TabView != null)
            {
                if (TabView.loading)
                    TabView.WebView.Browser.Stop();
                else
                    AddressBar.SetAddress(TabView.WebView.Browser.URL, TabView.certificates, TabView.certificateErrors);
                    TabView.WebView.Browser.Reload();
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (TabView != null)
            {
                mainWindow.MenuPopup.IsOpen = !mainWindow.MenuPopup.IsOpen;
            }
        }
    }
}
