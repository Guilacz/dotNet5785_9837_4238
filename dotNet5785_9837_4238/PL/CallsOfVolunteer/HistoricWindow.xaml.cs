using BO;
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
    /// Interaction logic for HistoricWindow.xaml
    /// </summary>
    public partial class HistoricWindow : Window
    {

        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();



        private int volunteerId;




        public IEnumerable<BO.ClosedCallInList> ListOfCalls
        {
            get { return (List<BO.ClosedCallInList>)GetValue(ListOfCallsProperty); }
            set { SetValue(ListOfCallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListOfCalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListOfCallsProperty =
            DependencyProperty.Register("ListOfCalls", typeof(List<BO.ClosedCallInList>), typeof(HistoricWindow));





        public HistoricWindow(int id)
        {
            InitializeComponent();
            volunteerId = id;
            Loaded += ClosedCallsHistory_loaded;


        }


        private void ClosedCallsHistory_loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var closedCalls = s_bl.Call.GetListOfClosedCall(volunteerId);

                if (closedCalls == null || !closedCalls.Any())
                {
                    MessageBox.Show("No calls found for the specified volunteer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ListOfCalls = closedCalls;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}