using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LeanBrowser.Controls
{
    /// <summary>
    /// Interaction logic for IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();
        }

        public void SetIcon(string key)
        {
            var source = (ImageSource)Application.Current.Resources[key];
            image.Source = source;
        }
    }
}
