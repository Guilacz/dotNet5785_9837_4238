using BO;
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
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow));


        public BO.CallType CallTypeSelected { get; set; } = BO.CallType.None;

       

        private void ComboBoxCallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VolunteerList = (CallTypeSelected == BO.CallType.None) ?
            s_bl?.Volunteer.GetVolunteerInLists()! : s_bl?.Volunteer.GetVolunteerInLists(null, BO.VolunteerSortField.CallType)!;
        }

        private void queryVolunteerList()
            => VolunteerList = (CallTypeSelected == BO.CallType.None) ?
            s_bl?.Volunteer.GetVolunteerInLists()! : s_bl?.Volunteer.GetVolunteerInLists(null, BO.VolunteerSortField.CallType)!;

        private void VolunteerListObserver()
            => queryVolunteerList();
 
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);




        /// <summary>
        /// link between the selected element to its details
        /// </summary>
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.VolunteerId).Show();
        }


        /// <summary>
        /// delete function
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int volunteerId)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the volunteer with ID {volunteerId}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.Delete(volunteerId);

                        queryVolunteerList();

                        MessageBox.Show($"Volunteer with ID {volunteerId} has been successfully deleted.",
                                        "Deletion Successful",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                    catch (BO.BlDeletionImpossible ex)
                    {
                        MessageBox.Show(ex.Message, "Deletion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (BO.BlInvalidValueException ex)
                    {
                        MessageBox.Show(ex.Message, "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }





        public VolunteerListWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded; // Charge les données au démarrage
            Closed += Window_Closed;

            queryVolunteerList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow().Show();
        }
    }
}
