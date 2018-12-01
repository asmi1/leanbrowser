using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DotNetBrowser;

namespace LeanBrowser
{
    public class BrowserPrintHandler : PrintHandler
    {
        public PrintStatus OnPrint(PrintJob printJob)
        {
            ManualResetEvent waitEvent = new ManualResetEvent(false);
            PrintDialog dialog = null;
            bool? print = false;
            PrintSettings settings = printJob.PrintSettings;

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                dialog = new PrintDialog();
                print = dialog.ShowDialog();

                if (print != null && print.Value)
                {
                    settings.PrinterName = dialog.PrintQueue.Name;

                    if (dialog.PageRangeSelection == PageRangeSelection.UserPages)
                    {
                        settings.PageRanges = new List<DotNetBrowser.PageRange>()
                    {
                        new DotNetBrowser.PageRange(dialog.PageRange.PageFrom, dialog.PageRange.PageTo)
                    };
                    }

                    if (dialog.PrintTicket.CopyCount != null)
                        settings.Copies = dialog.PrintTicket.CopyCount.Value;

                    if (dialog.PrintTicket.Duplexing != null)
                        settings.DuplexMode = (DuplexMode)((int)dialog.PrintTicket.Duplexing + 1);
                }

                waitEvent.Set();
            }));

            waitEvent.WaitOne();

            return print != null && print.Value
                    ? PrintStatus.CONTINUE
                    : PrintStatus.CANCEL;
        }
    }
}
