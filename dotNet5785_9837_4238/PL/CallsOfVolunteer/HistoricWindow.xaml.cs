//using BO;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;


using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.CallsOfVolunteer
{
    /// <summary>
    /// Interaction logic for HistoricWindow.xaml
    /// </summary>
    public partial class HistoricWindow : Window
    {
        // Access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        //---------------------Properties/ Dependency properties---------------------------


        // DependencyProperty for the list of closed calls
        public IEnumerable<BO.ClosedCallInList> ListOfCalls
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ListOfCallsProperty); }
            set { SetValue(ListOfCallsProperty, value); }
        }
        public static readonly DependencyProperty ListOfCallsProperty =
            DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.ClosedCallInList>), typeof(HistoricWindow));


        // Selected sort field
        public BO.CloseCallInListSort SortSelected { get; set; } = BO.CloseCallInListSort.None;


        // Selected filter type
        public BO.CallType CallTypeSelected { get; set; } = BO.CallType.None;


        // The ID of the volunteer for which the history is displayed
        private int _volunteerId;



        //---------------------FUNCTIONS---------------------------


        /// <summary>
        /// Constructor for HistoricWindow
        /// </summary>
        /// <param name="volunteerId">ID of the volunteer</param>
        public HistoricWindow(int volunteerId)
        {
            if (volunteerId <= 0)
                throw new ArgumentException("Invalid volunteer ID.", nameof(volunteerId));
            _volunteerId = volunteerId;
            InitializeComponent();
            
            // Load the data and set up observers
            queryClosedCalls();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        /// <summary>
        /// Loads the closed calls for the specified volunteer
        /// </summary>
        private void queryClosedCalls()
        {
            try
            {
                // Query based on the selected sort and filter
                ListOfCalls = s_bl.Call.GetListOfClosedCall(_volunteerId, CallTypeSelected == BO.CallType.None ? null : CallTypeSelected, SortSelected == BO.CloseCallInListSort.None ? null : SortSelected);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ListOfCalls = Enumerable.Empty<BO.ClosedCallInList>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ListOfCalls = Enumerable.Empty<BO.ClosedCallInList>();
            }
        }

        /// <summary>
        /// Observer to actualize the list of calls
        /// </summary>
        private void ClosedCallsObserver() => queryClosedCalls();


        /// <summary>
        /// Event handler for when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(ClosedCallsObserver);
        }

        /// <summary>
        /// Event handler for when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(ClosedCallsObserver);
        }

        /// <summary>
        /// Event handler for the sort combobox selection change
        /// </summary>
        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryClosedCalls();
        }

        /// <summary>
        /// Event handler for the filter combobox selection change
        /// </summary>
        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryClosedCalls();
        }


        //---------------------BUTTONS---------------------------


        /// <summary>
        /// Return button click event
        /// </summary>
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var volunteerWindow = new MainVolunteerWindow(_volunteerId);
             volunteerWindow.Show();
            this.Close();
        }
    }
}


