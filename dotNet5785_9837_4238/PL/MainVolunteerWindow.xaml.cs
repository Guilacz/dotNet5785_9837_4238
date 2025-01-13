using PL.CallsOfVolunteer;
using PL.Volunteer;
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
    /// Interaction logic for MainVolunteerWindow.xaml
    /// </summary>
    public partial class MainVolunteerWindow : Window
    {

        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        /// <summary>
        /// dependency property of the current volunteer
        /// </summary>
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));


        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            // כאן נקרא לפונקציה שמחזירה סיסמה אקראית
            string generatedPassword = s_bl.Volunteer.NewPassword();

            // מעדכנים את ה-TextBox
            CurrentVolunteer.Password = generatedPassword;
        }


        public BO.CallInProgress CurrentCall
        {
            get { return (BO.CallInProgress?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.CallInProgress), typeof(MainVolunteerWindow));






        /// <summary>
        /// This method calls the volunteer observer.
        /// </summary>
        private void volunteerObserver()
        {
            int id = CurrentVolunteer!.VolunteerId;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.Read(id);


        }

        /// <summary>
        /// This method loads the window.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.VolunteerId != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
        }

        /// <summary>
        /// This method closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.VolunteerId, volunteerObserver);
        }




        public MainVolunteerWindow(int id)
        {

            InitializeComponent();
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            CurrentCall = CurrentVolunteer?.callInCaring ?? new BO.CallInProgress(); // Initialisez avec un objet par défaut

            Loaded += Window_Loaded;

            Closed += Window_Closed;

        }


        //------------------------BUTTONS------------------------//

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                s_bl.Volunteer.Update(CurrentVolunteer!.VolunteerId, CurrentVolunteer!);
                MessageBox.Show("The volunteer has been successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            new HistoricWindow(CurrentVolunteer!.VolunteerId).Show();
            this.Close();
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            new ChooseCallWindow(CurrentVolunteer!.VolunteerId).Show();
            this.Close();

        }

        private void CancellCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.UpdateCallCancelled(CurrentVolunteer!.VolunteerId, CurrentVolunteer.callInCaring!.Id);
                MessageBox.Show("The call has been successfully cancelled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while cancelling the call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.UpdateCallFinished(CurrentVolunteer!.VolunteerId, CurrentVolunteer.callInCaring!.Id);
                MessageBox.Show("The call has been successfully finished.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while finishing the call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}