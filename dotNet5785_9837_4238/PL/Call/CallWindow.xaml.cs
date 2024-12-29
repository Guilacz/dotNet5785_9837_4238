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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();



        public string ButtonText2
        {
            get { return (string)GetValue(ButtonText2Property); }
            set { SetValue(ButtonText2Property, value); }
        }
        public static readonly DependencyProperty ButtonText2Property =
            DependencyProperty.Register("ButtonText2", typeof(string), typeof(CallWindow));



        public BO.Call CurrentCall
        {
            get { return (BO.Call)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow));



        public CallWindow(int Id = 0)
        {
            ButtonText2 = Id == 0 ? "Add" : "Update";

            InitializeComponent();

            if (Id != 0)
            {
                try
                {
                    CurrentCall= s_bl.Call.Read(Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while fetching call data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                CurrentCall = new BO.Call();
            }

        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText2 == "Add")
            {
                try
                {
                    s_bl.Call.Create(CurrentCall!);
                    MessageBox.Show("The call has been successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (ButtonText2 == "Update")
            {
                try
                {
                    s_bl.Call.Update( CurrentCall);
                    MessageBox.Show("The call has been successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            this.Close();
        }

    }
}

