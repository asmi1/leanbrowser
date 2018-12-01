using System.Windows;

namespace LeanBrowser
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        public DialogBox()
        {
            InitializeComponent();
            ShowInTaskbar = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
