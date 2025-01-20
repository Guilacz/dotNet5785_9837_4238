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
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        //---------------------Dependency properties---------------------------


        //dependency property for the text of the button
        public string ButtonText2
        {
            get { return (string)GetValue(ButtonText2Property); }
            set { SetValue(ButtonText2Property, value); }
        }

        public static readonly DependencyProperty ButtonText2Property =
            DependencyProperty.Register("ButtonText2", typeof(string), typeof(CallWindow));

        //dependency property for the current call
        public BO.Call CurrentCall
        {
            get { return (BO.Call)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow));


        //dependency property for the current assignment
        public BO.CallAssignInList? SelectedAssignment
        {
            get { return (BO.CallAssignInList)GetValue(SelectedAssignmentProperty); }
            set { SetValue(SelectedAssignmentProperty, value); }
        }

        public static readonly DependencyProperty SelectedAssignmentProperty =
            DependencyProperty.Register("SelectedAssignment", typeof(BO.CallAssignInList), typeof(CallWindow));


        //---------------------FUNCTIONS---------------------------

        private void CallObserver()
        {
            int id = CurrentCall!.CallId;
            CurrentCall = null;
            CurrentCall = s_bl.Call.Read(id);

        }

        /// <summary>
        /// Event handler for when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CurrentCall.CallId, CallObserver);
        }


        /// <summary>
        /// Event handler for when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall != null && CurrentCall.CallId != 0)
            {
                s_bl.Call.RemoveObserver(CurrentCall.CallId, CallObserver);
            }
        }

        /// <summary>
        /// Constructor of the window
        /// </summary>
        public CallWindow(int Id = 0)
        {
            ButtonText2 = Id == 0 ? "Add" : "Update";

            InitializeComponent();

            if (Id != 0)
            {
                try
                {
                    CurrentCall = s_bl.Call.Read(Id);
                    if (CurrentCall != null && CurrentCall.CallId != 0)
                    {
                        s_bl.Call.AddObserver(CurrentCall.CallId, CallObserver);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while fetching call data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                CurrentCall = new BO.Call();
            }

        }


        //---------------------BUTTONS---------------------------

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText2 == "Add")
            {
                try
                {
                    s_bl.Call.Create(CurrentCall!);
                    MessageBox.Show("The call has been successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (ButtonText2 == "Update")
            {
                try
                {
                    s_bl.Call.Update(CurrentCall);
                    MessageBox.Show("The call has been successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

