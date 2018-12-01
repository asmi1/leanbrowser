using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeanBrowser.Controls
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : UserControl
    {
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(DialogBox));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public DialogBox()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
