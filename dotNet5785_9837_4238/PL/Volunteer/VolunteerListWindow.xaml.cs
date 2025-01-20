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


        //---------------------Properties/ Dependency properties---------------------------


        //DependencyProperty of the Volunteer list
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow));


        /// <summary>
        /// selected call type in the combobox
        /// </summary>
        public BO.CallType CallTypeSelected { get; set; } = BO.CallType.None;
        
        /// <summary>
        /// selected volunteer on the grid
        /// </summary>
        public BO.VolunteerInList? SelectedVolunteer { get; set; }


        //---------------------FUNCTIONS---------------------------


        //private void queryVolunteerList()
        //    => VolunteerList = (CallTypeSelected == BO.CallType.None) ?
        //    s_bl?.Volunteer.GetVolunteerInLists()! : s_bl?.Volunteer.GetVolunteerInLists(null, BO.VolunteerSortField.CallType)!;

        //private void VolunteerListObserver()
        //    => queryVolunteerList();

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //    => s_bl.Volunteer.AddObserver(VolunteerListObserver);

        //private void Window_Closed(object sender, EventArgs e)
        //    => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        private void queryVolunteerList()
        {
            Dispatcher.Invoke(() =>
            {
                VolunteerList = (CallTypeSelected == BO.CallType.None) ?
                    s_bl?.Volunteer.GetVolunteerInLists()! :
                    s_bl?.Volunteer.GetVolunteerInLists(null, BO.VolunteerSortField.CallType)!;
            });
        }

        private void VolunteerListObserver()
        {
            Dispatcher.Invoke(() => queryVolunteerList());
        }

        /// <summary>
        /// Event handler for when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);  // רישום להשקפה על כל הרשימה
        }

        /// <summary>
        /// Event handler for when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);  // הסרת הרישום להשקפה על הרשימה
        }







        /// <summary>
        /// constructor of the volunteer list window
        /// </summary>
        public VolunteerListWindow()
        {
            InitializeComponent();
            queryVolunteerList();
            Loaded += Window_Loaded;
            Closed += Window_Closed;

        }







        ///-----------------------------------------BUTTONS------------------------------------

        /// <summary>
        /// delete function of the button
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

        /// <summary>
        /// FUNCTION of when we change the selection in the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxCallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallTypeSelected == BO.CallType.None)
            {
                IEnumerable<VolunteerInList> VolunteerList = s_bl?.Volunteer.GetVolunteerInLists()!;
            }
            else
            {
                VolunteerList = s_bl?.Volunteer.GetVolunteersListByCallType(CallTypeSelected)!;
            }
        }

        /// <summary>
        /// function of when we double click on a volunteer
        /// </summary>
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                BO.Volunteer vol = s_bl.Volunteer.Read(SelectedVolunteer.VolunteerId);
                new VolunteerWindow(vol.VolunteerId).Show();
            }
              
                
        }


        /// <summary>
        /// add  button
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
                new VolunteerWindow().Show();
        }

    }
}

