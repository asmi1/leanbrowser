using DotNetBrowser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace LeanBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
 
        [STAThread]
        public static void Main(string[] args)
        {
            var proc = Process.GetCurrentProcess();
            var processName = proc.ProcessName.Replace(".vshost", "");
            var runningProcess = Process.GetProcesses()
                .FirstOrDefault(x => (x.ProcessName == processName ||
                                x.ProcessName == proc.ProcessName ||
                                x.ProcessName == proc.ProcessName + ".vshost") && x.Id != proc.Id);

            if (runningProcess == null)
            {
                var app = new App();
                app.InitializeComponent();
                MainWindow mainWindow = new MainWindow(args);
                app.Run(mainWindow);
                return; // In this case we just proceed on loading the program
            } else
            {
                UnsafeNative.SendMessage(runningProcess.MainWindowHandle, string.Join(" ", args));
            }
        }
    }
}
