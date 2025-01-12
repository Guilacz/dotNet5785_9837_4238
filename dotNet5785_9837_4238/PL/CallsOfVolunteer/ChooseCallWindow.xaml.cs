using BO;
using PL.Call;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PL.CallsOfVolunteer
{
    /// <summary>
    /// Interaction logic for ChooseCallWindow.xaml
    /// </summary>
    public partial class ChooseCallWindow : Window
    {
        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        private int volunteerId;

        public IEnumerable<BO.Call> ListOfCalls
        {
            get { return (IEnumerable<BO.Call>)GetValue(ListOfCallsProperty); }
            set { SetValue(ListOfCallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListOfCalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListOfCallsProperty =
            DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.Call>), typeof(ChooseCallWindow));

        public BO.CallType CallTypeSelected { get; set; } = BO.CallType.None;
        public OpenCallInListSort SortSelected { get; set; } = OpenCallInListSort.None;



        public ChooseCallWindow(int id)
        {
            volunteerId = id;
            InitializeComponent();

            this.Loaded += ChooseCallWindow_Loaded;

        }


        private void ChooseCallWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Fetch initial list of calls
                var callIds = s_bl.Call.GetListOfCalls()
                    .Where(call => call.CallInListStatus == BO.CallInListStatus.Open
                                   || call.CallInListStatus == BO.CallInListStatus.OpenAtRisk)
                    .Select(call => call.CallId)
                    .ToList();


                // Fetch full details for each call using GetCallDetails
                var detailedCalls = callIds
                    .Select(callId => s_bl.Call.GetCallDetails(callId))
                    .ToList();

                ApplyFilterAndSort(detailedCalls);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterAndSort(List<BO.Call> calls)
        {
            if (CallTypeSelected != BO.CallType.None)
            {
                calls = calls.Where(call => call.CallType == CallTypeSelected).ToList();
            }

            switch (SortSelected)
            {
                case OpenCallInListSort.CallId:
                    calls = calls.OrderBy(call => call.CallId).ToList();
                    break;
                case OpenCallInListSort.CallType:
                    calls = calls.OrderBy(call => call.CallType).ToList();
                    break;
                case OpenCallInListSort.Address:
                    calls = calls.OrderBy(call => call.Address).ToList();
                    break;
                case OpenCallInListSort.OpenTime:
                    calls = calls.OrderBy(call => call.OpenTime).ToList();
                    break;
                case OpenCallInListSort.MaxTime:
                    calls = calls.OrderBy(call => call.MaxTime).ToList();
                    break;
                case OpenCallInListSort.Details:
                    calls = calls.OrderBy(call => call.Details).ToList();
                    break;
                case OpenCallInListSort.None:
                    // no filter
                    break;
            }

            ListOfCalls = calls;
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (SortSelected != null)
            {
                ApplyFilterAndSort(ListOfCalls.ToList());
            }
        }

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallTypeSelected != BO.CallType.None)
            {
                ApplyFilterAndSort(ListOfCalls.ToList());
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var volunteerWindow = new MainVolunteerWindow(volunteerId);
            volunteerWindow.Show();
            this.Close();

        }

        private void ChooseCallButton_Click(object sender, RoutedEventArgs e)
        {


        }


    }
}