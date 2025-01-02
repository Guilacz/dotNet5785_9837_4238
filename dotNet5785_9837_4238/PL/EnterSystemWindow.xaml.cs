using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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



        public string PasswordValue
        {
            get { return (string)GetValue(PasswordValueProperty); }
            set { SetValue(PasswordValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PasswordValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordValueProperty =
            DependencyProperty.Register("PasswordValue", typeof(string), typeof(EnterSystemWindow));






        public EnterSystemWindow()
        {
            InitializeComponent();
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {

                PasswordValue = passwordBox.Password;
            }
        }

       

        /// <summary>
        /// login function : checks the role and open the right window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TzValue) || string.IsNullOrWhiteSpace(PasswordValue))
            {
                MessageBox.Show("Please fill all the required fields.", "Missing Values", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (!int.TryParse(TzValue, out int userId))
                {
                    MessageBox.Show("Invalid ID format. Please enter numeric ID.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var role = s_bl.Volunteer.EnterSystemWithId(userId, PasswordValue);

                switch (role)
                {
                    case BO.Role.Volunteer:
                        var volunteerWindow = new VolunteerWindow();
                        volunteerWindow.Show();
                        this.Close();
                        break;

                    case BO.Role.Manager:
                        var managerWindow = new MainWindow();
                        managerWindow.Show();
                        this.Close();
                        break;

                    default:
                        MessageBox.Show("Unknown role. Please contact support.", "Unknown Role", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (BO.BlArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlInvalidValueException ex)
            {
                MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
