using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LeanBrowser.Controls
{
    /// <summary>
    /// Interaction logic for TextIconButton.xaml
    /// </summary>
    public partial class TextIconButton : UserControl
    {
        public TextIconButton()
        {
            InitializeComponent();
        }

        public void SetLabel(string text)
        {
            label.Text = text;
        }

        public void SetTip(string text)
        {
            tip.Text = text;
        }

        public void SetIcon(string key)
        {
            var source = (ImageSource)Application.Current.Resources[key];
            image.Source = source;
        }
    }
}
