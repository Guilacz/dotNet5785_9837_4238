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
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for EnterSystemWindow.xaml
    /// </summary>
    public partial class EnterSystemWindow : Window
    {

        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();



        public string TzValue
        {
            get { return (string)GetValue(TzValueProperty); }
            set { SetValue(TzValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TzValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TzValueProperty =
            DependencyProperty.Register("TzValue", typeof(string), typeof(EnterSystemWindow));




        public EnterSystemWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Vérifiez si TzValue contient une valeur
            if (!string.IsNullOrWhiteSpace(TzValue))
            {
                // Affichez la valeur dans une boîte de dialogue
                MessageBox.Show($"La valeur de T.Z est : {TzValue}", "Valeur T.Z", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Affichez une boîte de dialogue indiquant que la valeur est vide
                MessageBox.Show("La valeur de T.Z est vide.", "Valeur T.Z", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
