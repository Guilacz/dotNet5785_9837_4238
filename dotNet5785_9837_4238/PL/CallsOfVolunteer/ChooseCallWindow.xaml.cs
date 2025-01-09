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



        public List<BO.Call> ListOfCalls
        {
            get { return (List<BO.Call>)GetValue(ListOfCallsProperty); }
            set { SetValue(ListOfCallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListOfCalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListOfCallsProperty =
            DependencyProperty.Register("ListOfCalls", typeof(List<BO.Call>), typeof(ChooseCallWindow));



        public ChooseCallWindow(int id)
        {
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

                ListOfCalls = detailedCalls;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}