using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows.Threading;
using DotNetBrowser.Events;
using DotNetBrowser.WPF;
using DotNetBrowser.DOM.Events;
using DotNetBrowser.DOM;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using DotNetBrowser;
using System.Threading;
using System.Windows.Input;

namespace LeanBrowser
{
    /// <summary>
    /// Interaction logic for TabView.xaml
    /// </summary>
    public partial class TabView : UserControl
    {
        //Declarations
        private readonly MainWindow mainWindow;
        private string Title;
        private string LastWebsite;
        public bool loading;
        private DOMEventHandler audioPlayingEvent;
        public WPFBrowserView WebView;

        public Dictionary<string, CertificateErrorParams> certificateErrors;
        public Dictionary<string, Certificate> certificates;

        private TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        
        public string UrlToLoad { get; set; }

    public TabView(MainWindow mw, string url)
        {
            InitializeComponent();

            // Assignments
            mainWindow = mw;
            UrlToLoad = string.IsNullOrEmpty(url)? "google.com": url;
            LastWebsite = "";
            loading = false;
            certificateErrors = new Dictionary<string, CertificateErrorParams>();
            certificates = new Dictionary<string, Certificate>();

            Loaded += TabView_Loaded;

            // Add this page to main window
            mw.Pages.Add(this);
        }

        private async void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() =>
                SetupBrowser(), CancellationToken.None, TaskCreationOptions.None, uiScheduler);

            // Focus main window else KeyDown events won't be fired
            mainWindow.Focus();
        }

        private async void WebView_StartLoadingFrameEvent(object sender, StartLoadingArgs e)
        {
            if (e.IsMainFrame)
            {
                await Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            WebView_Loading();
                            if (IsVisible)
                            {
                                mainWindow.ToolBar.RefreshToolbar();
                            }

                            mainWindow.TabBar.getTabFromForm(this).StartLoading();

                            // Web Page is loading, hide the speaker icon in tab title
                            mainWindow.TabBar.getTabFromForm(this).SetAudioMuteVisibility(Visibility.Collapsed);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("WebView StartLoading error: " + ex.Message + " " + ex.Data);
                        }
                    }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        }
        
        /// <summary>
        /// DOM Content loaded but the page might still loading additional resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebView_ProvisionalLoadingFrameEvent(object sender, ProvisionalLoadingArgs e)
        {
            if (e.IsMainFrame)
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        WebView_Loaded();
                        if (IsVisible)
                        {
                            mainWindow.ToolBar.RefreshToolbar();
                            mainWindow.ToolBar.AddressBar.SetAddress(e.Url, certificates, certificateErrors);

                            // Update title if source code
                            if (WebView.Browser.URL.StartsWith("view-source:"))
                            {
                                mainWindow.TabBar.getTabFromForm(this).SetTitle(WebView.Browser.URL);
                                Title = WebView.Browser.URL;
                            }
                        }

                        //Task.Factory.StartNew(WriteHistory);
                        mainWindow.Menu.RefreshZoom();

                        // Registers listeners for HTML5 video/audio play/pause/ended events
                        DOMEvent play = WebView.Browser.CreateEvent("play");
                        DOMDocument document = e.Browser.GetDocument();
                        List<DOMNode> tags = document.GetElementsByTagName("audio").Concat(document.GetElementsByTagName("video")).ToList();
                        foreach (DOMNode tag in tags)
                        {
                            tag.AddEventListener(play, audioPlayingEvent, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("WebView ProvisionalLoading error: " + ex.Message + " " + ex.Data);
                    }
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        }

        private async void WebView_FinishLoadingFrameEvent(object sender, FinishLoadingEventArgs e)
        {
            if (e.IsMainFrame)
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (IsVisible)
                        {
                            mainWindow.ToolBar.RefreshToolbar();
                            if (!WebView.Browser.URL.StartsWith("view-source:"))
                            {
                                mainWindow.ToolBar.AddressBar.SetAddress(e.ValidatedURL, certificates, certificateErrors);
                            }
                        }

                        UpdateTitle(WebView.Browser.Title);
                        UpdateFavicon(e.ValidatedURL);

                        // Focus main window else KeyDown events won't be fired
                        mainWindow.Focus();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("WebView FinishLoading error: " + ex.Message + " " + ex.Data);
                    }
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        }

        private async void WebView_FailLoadingFrameEvent(object sender, FailLoadingEventArgs e)
        {
            if (e.IsMainFrame)
            {
                await Task.Factory.StartNew(() =>
                {
                    WebView_Loaded();

                    // Focus main window else KeyDown events won't be fired
                    mainWindow.Focus();
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        }

        private async void WebView_TitleChangedEvent(object sender, TitleEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                UpdateTitle(e.Title.ToString());
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private async void WebView_StatusChangedEvent(object sender, StatusEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                UpdateStatusBar(e.Text);
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void WebView_DOMAudioPlayingEventListener(object sender, DOMEventArgs e)
        {
            // Audio/Video is playing, display the speaker icon in tab title
            mainWindow.TabBar.getTabFromForm(this).SetAudioMuteVisibility(Visibility.Visible);
        }

        private void SetupBrowser()
        {
            // Setup browser
            Browser browser;

            if (mainWindow.isIncognito == true)
            {
                browser = BrowserFactory.Create(MainWindow.iContext, BrowserType.HEAVYWEIGHT);
            } else
            {
                browser = BrowserFactory.Create(BrowserContext.DefaultContext, BrowserType.HEAVYWEIGHT);
            }

            WebView = new WPFBrowserView(browser);
            BrowserContainer.Children.Add((UIElement)WebView.GetComponent());

            WebView.Browser.LoadURL(UrlToLoad);
            WebView.ClipToBounds = true;

            // Events
            WebView.Browser.StartLoadingFrameEvent += WebView_StartLoadingFrameEvent;
            WebView.Browser.ProvisionalLoadingFrameEvent += WebView_ProvisionalLoadingFrameEvent;
            WebView.Browser.FinishLoadingFrameEvent += WebView_FinishLoadingFrameEvent;
            WebView.Browser.FailLoadingFrameEvent += WebView_FailLoadingFrameEvent;
            WebView.Browser.TitleChangedEvent += WebView_TitleChangedEvent;
            WebView.Browser.StatusChangedEvent += WebView_StatusChangedEvent;

            // Forward KeyDown event to main window
            WebView.KeyDown += (sender, args) => OnKeyDown(args);

            // Handlers
            WebView.Browser.PopupHandler = new BrowserPopupHandler(mainWindow, this);
            WebView.Browser.FullScreenHandler = new BrowserFullScreenHandler(mainWindow);
            WebView.Browser.LoadHandler = new BrowserLoadHandler(mainWindow);
            WebView.Browser.Context.NetworkService.CertificateVerifier = new BrowserCertificateVerifier(mainWindow, WebView.Browser.URL);
            WebView.Browser.ContextMenuHandler = new BrowserContextMenuHandler(WebView, true);
            WebView.Browser.PrintHandler = new BrowserPrintHandler();

            audioPlayingEvent = WebView_DOMAudioPlayingEventListener;
        }

        private async void UpdateFavicon(string url)
        {
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Uri uri = new Uri(url);
                    string host = uri.Host;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(string.Format("https://www.google.com/s2/favicons?domain_url={0}", host), UriKind.Absolute);
                    bitmap.EndInit();

                    mainWindow.TabBar.getTabFromForm(this).SetIcon(bitmap);
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Favicon load error: " + ex.Message + " " + ex.Data);
            }
        }

        private void WebView_Loading()
        {
            loading = true;
            mainWindow.TabBar.getTabFromForm(this).StartLoading();
            UpdateStatusBar(string.Format("Loading {0} ..", WebView.Browser.URL));
        }

        private void WebView_Loaded()
        {
            loading = false;
            mainWindow.TabBar.getTabFromForm(this).StopLoading();
            UpdateStatusBar("");
        }

        public void Shutdown()
        {
            WebView.Dispose();
            WebView.Browser.Dispose();
        }

        public void UpdateTitle(string title)
        {
            mainWindow.TabBar.getTabFromForm(this).SetTitle(title);
            Title = title;
        }

        public async Task WriteHistory()
        {
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    if (!LastWebsite.Equals(WebView.Browser.URL))
                    {
                        if (!File.Exists(StaticDeclarations.Historypath))
                        {
                            var history = new HistItem();
                            history.Title = Title;
                            history.Url = WebView.Browser.URL;
                            var histories = new List<HistItem>();
                            histories.Add(history);
                            var newJson = JsonConvert.SerializeObject(histories);
                            File.WriteAllText(StaticDeclarations.Historypath, newJson);

                            LastWebsite = WebView.Browser.URL;
                        }
                        else
                        {
                            HistItem history = new HistItem();
                            history.Title = Title;
                            history.Url = WebView.Browser.URL;
                            string json = File.ReadAllText(StaticDeclarations.Historypath);
                            List<HistItem> histories = JsonConvert.DeserializeObject<List<HistItem>>(json);
                            histories.Add(history);
                            string newJson = JsonConvert.SerializeObject(histories);
                            File.WriteAllText(StaticDeclarations.Historypath, newJson);

                            LastWebsite = WebView.Browser.URL;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Write history error: " + ex.Message + " " + ex.Data);
                }
            }));
        }

        public async void UpdateStatusBar(string text)
        {
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                mainWindow.statusBar.Text = text;
            }));
        }
    }
}
