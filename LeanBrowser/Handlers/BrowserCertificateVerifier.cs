using DotNetBrowser;
using System;
using System.Windows;

namespace LeanBrowser
{
    class BrowserCertificateVerifier : CertificateVerifier
    {
        private string requestURL;
        private TabView tabView;
        private MainWindow mainWindow;

        public BrowserCertificateVerifier(MainWindow mw, string url)
        {
            requestURL = url;
            mainWindow = mw;
            tabView = mainWindow.getActiveTabView();
        }

        public CertificateVerifyResult Verify(CertificateVerifyParams parameters)
        {
            Uri uri = new Uri(requestURL);
            string host = uri.Host;

            if (!tabView.certificates.ContainsKey(host))
            {
                tabView.certificates.Add(host, parameters.Certificate);
            }
            return CertificateVerifyResult.DEFAULT;
        }
    }
}
