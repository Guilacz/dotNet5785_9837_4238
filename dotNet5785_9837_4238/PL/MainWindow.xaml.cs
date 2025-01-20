using PL.Call;
using PL.Volunteer;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();






        //-------------------------Dependency properties-------------------------------------------------

        /// <summary>
        /// dependency property of CurrentTime
        /// </summary>
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(DateTime.Now));


        /// <summary>
        /// dependency property of riskRange
        /// </summary>
        public TimeSpan RiskRangeProperty
        {
            get { return (TimeSpan)GetValue(RiskRangePropertyProperty); }
            set { SetValue(RiskRangePropertyProperty, value); }
        }
        public static readonly DependencyProperty RiskRangePropertyProperty =
            DependencyProperty.Register("RiskRangeProperty", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));



        //-------------------------FUNCTIONS-------------------------------------------------

        //observers
        private void ClockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void ConfigObserver()
        {
            RiskRangeProperty = s_bl.Admin.GetMaxRange();
        }

        //constructor of the window
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnMainWindow_Loaded;
        }

        /// <summary>
        /// function to update the riskRange
        /// </summary>
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetMaxRange(RiskRangeProperty);
        }


        /// <summary>
        /// Event handler for when the window is loaded
        /// </summary>
        private void OnMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
                CurrentTime = s_bl.Admin.GetClock();

                RiskRangeProperty = s_bl.Admin.GetMaxRange();

                s_bl.Admin.AddClockObserver(ClockObserver);    
                s_bl.Admin.AddConfigObserver(ConfigObserver);  
        }


        /// <summary>
        /// function to delete ClockObserver and ConfigObserver when we close the window
        /// </summary>
        private void OnMainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);

        }




        //---------------------------------------------------------------------------BUTTONS--------------------------------------------------


        //BUTTONS TO ADD TIME------------------------
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.minute);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.hour);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.day);
        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.month);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.year);
        }

        //------------------------------------


        /// <summary>
        /// function to open a screen of the list of volunteers
        /// </summary>
        private void btnVolunteersList_Click(object sender, RoutedEventArgs e)
        { 
            new VolunteerListWindow().Show(); 
        }



        /// <summary>
        /// function to open a screen of the list of calls
        /// </summary>
        private void btnCallsList_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow().Show();
        }



        //Button to reset the database

        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to reset the database ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    Mouse.OverrideCursor = Cursors.Wait;
                    s_bl.Admin.ResetDB();
                    Mouse.OverrideCursor = null;

                    MessageBox.Show("Database reset succeeded.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = null; // Make sure the cursor is reset
                    MessageBox.Show($"Database reset failed.\nError: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        //button to initialize the database
        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
            "Are you sure you want to initialize the database ?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    Mouse.OverrideCursor = Cursors.Wait;
                    s_bl.Admin.InitializeDB();
                    Mouse.OverrideCursor = null;

                    MessageBox.Show("Database initialization succeeded.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = null; // Make sure the cursor is reset
                    MessageBox.Show($"Database initialization failed.\nError: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

    }
}