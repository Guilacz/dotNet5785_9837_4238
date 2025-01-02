using BO;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {

        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();



        //DependencyProperty of the call list
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow));


        public BO.CallInList? SelectedCall { get; set; }


       /// <summary>
       /// functions to get all the calls
       /// </summary>
        private void queryCallList()
        {
           CallList = s_bl?.Call.GetListOfCalls()!;   
        }

        private void CallListObserver()
            => queryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallListObserver);



        /// <summary>
        /// constructor of the call list window
        /// </summary>
        public CallListWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded; 
            Closed += Window_Closed;
            queryCallList();

        }



        ///-----------------------------------------BUTTONS------------------------------------


        /// <summary>
        /// add  button
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        /// <summary>
        /// function of when we double click on a volunteer
        /// </summary>
        private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new CallWindow(SelectedCall.CallId).Show();
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int callId)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the call with ID {callId}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.Delete(callId);

                        queryCallList();

                        MessageBox.Show($"call with ID {callId} has been successfully deleted.",
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
    }
}
