using DotNetBrowser;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LeanBrowser.Controls
{
    /// <summary>
    /// Interaction logic for AddressBar.xaml
    /// </summary>
    public partial class AddressBar : UserControl
    {

        public AddressBar()
        {
            InitializeComponent();

            // We need Dispatcher to set Focus on start
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(delegate () {
                    textBox.ContextMenu = null;
                    textBox.Focus();             // Set Logical Focus
                    Keyboard.Focus(textBox);     // Set Keyboard Focus
                })
            );
        }

        public void SetAddress(string URL, Dictionary<string, Certificate> certificates = null, Dictionary<string, CertificateErrorParams> certificateErrors = null)
        {
            // Brushes
            Brush grey = Brushes.DarkGray;
            Brush green = Brushes.Green;
            Brush red = Brushes.Red;

            Dispatcher.Invoke((Action)(() =>
            {
                // Make sure all labels are hidden and empty by default
                Clear_All_Labels();
                error.Text = "";
                errorCode.Text = "";

                error.Visibility = Visibility.Collapsed;
                CertificateInfo.IsEnabled = false;
                CertificateTooltip.IsOpen = false;

                scheme.TextDecorations = null;

                // Set editable address bar
                textBox.Text = URL;

                // Save original address
                original.Content = URL;

                Uri uri = new Uri(URL);

                // URL is HTTP or HTTPS
                if (uri.Scheme.Equals("https") | uri.Scheme.Equals("http"))
                {
                    scheme.Text = uri.Scheme;
                    delimiter.Text = Uri.SchemeDelimiter;
                    host.Text = uri.Host;
                    query.Text = uri.AbsolutePath;

                    if (uri.Scheme.Equals("https"))
                    {
                        scheme.Foreground = green;
                        icon.Source = (ImageSource)Application.Current.Resources["lockedGreenDrawingImage"];

                        if (certificateErrors != null && certificateErrors.ContainsKey(uri.Host))
                        {
                            // Host has invalid certificate
                            scheme.Foreground = red;
                            scheme.TextDecorations = TextDecorations.Strikethrough;
                            icon.Source = (ImageSource)Application.Current.Resources["warningRedDrawingImage"];

                            error.Text = "unsafe";
                            errorCode.Text = certificateErrors[uri.Host].CertificateError.ToString();
                            error.Visibility = Visibility.Visible;
                            CertificateInfo.IsEnabled = true;
                        }
                    }
                    else
                    {
                        scheme.Foreground = grey;
                        icon.Source = (ImageSource)Application.Current.Resources["unlockedDrawingImage"];
                    }
                }
                else
                {
                    host.Text = URL;
                    scheme.Foreground = grey;
                    icon.Source = (ImageSource)Application.Current.Resources["unlockedDrawingImage"];
                }

                addressBarBorder.Visibility = Visibility.Collapsed;
                placeholder.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Visible;
                formattedAddressBarBorder.Visibility = Visibility.Visible;
            }));
        }

        public void SetInvalidAddress()
        {
            Brush red = Brushes.Red;
            Dispatcher.Invoke((Action)(() =>
            {
                scheme.Foreground = red;
                icon.Source = (ImageSource)Application.Current.Resources["warningRedDrawingImage"];
            }));
        }

        public void ResetAddress()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Clear_All_Labels();
                error.Text = "";
                errorCode.Text = "";

                error.Visibility = Visibility.Collapsed;
                CertificateInfo.IsEnabled = false;
                CertificateTooltip.IsOpen = false;

                scheme.TextDecorations = null;

                textBox.Text = "";
                original.Content = "";
                icon.Source = (ImageSource)Application.Current.Resources["searchDrawingImageGray"];

                addressBarBorder.Visibility = Visibility.Visible;
                placeholder.Visibility = Visibility.Visible;
                textBox.Visibility = Visibility.Visible;
                formattedAddressBarBorder.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Buttons & address bar event handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dynamic textArray = textBox.Text.Split();
                ToolBar toolBar = this.FindParent<ToolBar>();
                if (toolBar == null)
                {
                    return;
                }

                string url = textBox.Text;

                if (!(textBox.Text.Contains(".") && !textBox.Text.Contains(" ") && !textBox.Text.Contains(" .") &&
                     !textBox.Text.Contains(". ")) && !textArray[0].Contains(":/") && !textArray[0].Contains(":\\"))
                {
                    string se = "Google";
                    if (se == "Google")
                    {
                        url = "http://google.com/#q=";
                    }
                    else if (se == "Bing")
                    {
                        url = "http://www.bing.com/search?q=";
                    }
                    else if (se == "DuckDuckGo")
                    {
                        url = "https://duckduckgo.com/?q=";
                    }

                    url += textBox.Text;
                    textBox.Text = url;
                }

                toolBar.TabView.UrlToLoad = url;
                toolBar.TabView.WebView.Browser.LoadURL(url);
            }
        }

        private void Clear_All_Labels()
        {
            scheme.Text = "";
            delimiter.Text = "";
            host.Text = "";
            query.Text = "";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.Equals(original.Content) & !string.IsNullOrEmpty(host.Text.ToString()))
            {
                addressBarBorder.Visibility = Visibility.Collapsed;
                formattedAddressBarBorder.Visibility = Visibility.Visible;
            } else
            {
                addressBarBorder.Visibility = Visibility.Visible;
                formattedAddressBarBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            System.Windows.Controls.ContextMenu popupMenu = new System.Windows.Controls.ContextMenu();

            // Undo
            popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Undo", textBox.CanUndo, Visibility.Visible, delegate
            {
                textBox.Undo();
            }));

            // Redo
            popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Redo", textBox.CanRedo, Visibility.Visible, delegate
            {
                textBox.Redo();
            }));

            // Separator
            popupMenu.Items.Add(new Separator());

            // Cut
            popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Cut", !string.IsNullOrEmpty(textBox.SelectedText), Visibility.Visible, delegate
            {
                textBox.Cut();
            }));

            // Copy
            popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Copy", !string.IsNullOrEmpty(textBox.SelectedText), Visibility.Visible, delegate
            {
                textBox.Copy();
            }));

            // Paste
            IDataObject ClipboardData = Clipboard.GetDataObject();
            bool canPaste = ClipboardData.GetDataPresent(DataFormats.UnicodeText)
                        || ClipboardData.GetDataPresent(DataFormats.Text)
                        || ClipboardData.GetDataPresent(DataFormats.Html);

            popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Paste", canPaste, Visibility.Visible, delegate
            {
                textBox.Paste();
            }));

            // Select All
            popupMenu.Items.Add(StaticFunctions.BuildContextMenuItem("Select all", string.IsNullOrEmpty(textBox.SelectedText), Visibility.Visible, delegate
            {
                textBox.SelectAll();
            }));

            // Display the context menu
            popupMenu.IsOpen = true;
        }

        private void FormattedAddressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            formattedAddressBarBorder.Visibility = Visibility.Collapsed;
            addressBarBorder.Visibility = Visibility.Visible;
            textBox.Focus();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!textBox.Text.Equals(original.Content))
            {
                Clear_All_Labels();
                formattedAddressBarBorder.Visibility = Visibility.Collapsed;
                addressBarBorder.Visibility = Visibility.Visible;
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                placeholder.Visibility = Visibility.Visible;
            } else
            {
                placeholder.Visibility = Visibility.Collapsed;
            }
        }

        private void CertificateInfo_Click(object sender, RoutedEventArgs e)
        {
            CertificateTooltip.IsOpen = !CertificateTooltip.IsOpen;
        }

        private void AddressBarControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
