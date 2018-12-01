using System.Windows.Controls;

namespace LeanBrowser
{
    public class OpenTabCommandParameters
    {
        public string Url { get; private set; }
        public string Title { get; private set; }
        public UserControl Control { get; private set; }

        public OpenTabCommandParameters(string url, string title)
        {
            Url = url;
            Title = title;
        }

        public OpenTabCommandParameters(UserControl control, string title)
        {
            Control = control;
            Title = title;
        }
        public OpenTabCommandParameters(UserControl control, string url, string title)
        {
            Control = control;
            Title = title;
            Url = url;
        }
    }
}
