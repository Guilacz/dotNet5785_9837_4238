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
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


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





        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(0));


        /// <summary>
        /// dependency property of riskRange
        /// </summary>
        public TimeSpan RiskRangeProperty
        {
            get { return (TimeSpan)GetValue(RiskRangePropertyProperty); }
            set { SetValue(RiskRangePropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RiskRangeProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RiskRangePropertyProperty =
            DependencyProperty.Register("RiskRangeProperty", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(0));

       



        /// <summary>
        /// 
        /// </summary>
        private void UpdateRiskRangeButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetMaxRange(RiskRangeProperty);

        }


        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
                CurrentTime = s_bl.Admin.GetClock();

                RiskRangeProperty = s_bl.Admin.GetMaxRange();

                s_bl.Admin.AddClockObserver(ClockObserver);    
                s_bl.Admin.AddConfigObserver(ConfigObserver);  
            
        }

        private void ClockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void ConfigObserver()
        {
            RiskRangeProperty = s_bl.Admin.GetMaxRange();
        }


        public MainWindow()
        {
            InitializeComponent();

        }


    }
}