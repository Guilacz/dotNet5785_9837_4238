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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {

        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        /// <summary>
        /// dependency property of the add/update button
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));


        /// <summary>
        /// dependency property of the current volunteer
        /// </summary>
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow));



        /// <summary>
        /// definition of the comboBox RoleType
        /// </summary>
        public BO.Role RoleTypeSelected { get; set; } = BO.Role.Volunteer;


        public VolunteerWindow(int volunteerId = 0)
        {
            ButtonText = volunteerId == 0 ? "Add" : "Update";

            InitializeComponent();

            if (volunteerId != 0)
            {
                try
                {
                    CurrentVolunteer = s_bl.Volunteer.Read(volunteerId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while fetching volunteer data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                CurrentVolunteer = new BO.Volunteer();
            }

            DataContext = this;


        }







        /// <summary>
        /// function of the AddUpdate button
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText == "Add")
            {
                try
                {
                    s_bl.Volunteer.Create(CurrentVolunteer!);
                    MessageBox.Show("The volunteer has been successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (ButtonText == "Update")
            {
                try
                {
                    // Appel à la méthode de mise à jour de l'entité
                    s_bl.Volunteer.Update(CurrentVolunteer.VolunteerId, CurrentVolunteer!);
                    MessageBox.Show("The volunteer has been successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
