using DotNetBrowser;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LeanBrowser
{
    public class BrowserLoadHandler : DefaultLoadHandler
    {
        private bool result;
        private TabView tabView;
        private MainWindow mainWindow;

        public BrowserLoadHandler(MainWindow mw)
        {
            mainWindow = mw;
            tabView = mainWindow.getActiveTabView();
        }

        public override bool OnCertificateError(CertificateErrorParams errorParams)
        {
            result = true; // By default, block sites with certificate error

            Uri uri = new Uri(errorParams.RequestURL);
            string host = uri.Host;

            if (!tabView.certificateErrors.ContainsKey(host))
            {
                tabView.certificateErrors.Add(host, errorParams);
            }

            mainWindow.Dispatcher.Invoke(() => result = CertificateDialogResponse(errorParams));
            
            return result;
        }

        private bool CertificateDialogResponse(CertificateErrorParams errorParams)
        {
            DialogBox dialog = new DialogBox
            {
                Owner = mainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dialog.msgTitle.Text = "Your connection is not private";
            dialog.Title = "Certificate Error";
            dialog.ok.Content = "Continue anyway";
            dialog.cancel.Content = "Go back";

            dialog.msgDesc.Text = "The website you are trying to visit is not secure. Attackers might be trying to fool you or steal your sensitive information. You should go back immediately.";
            dialog.msgDesc.Visibility = Visibility.Visible;

            TextBlock errorCode = new TextBlock();
            errorCode.Text = errorParams.CertificateError.ToString();
            errorCode.Margin = new Thickness(0, 10, 0, 0);
            errorCode.FontSize = 16;

            dialog.msgWrap.Children.Add(errorCode);

            dialog.Height = 280;
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                return false;  // Visit site
            }

            return true; // Block site
        }
    }
}
