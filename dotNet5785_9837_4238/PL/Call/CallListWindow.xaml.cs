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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    //public partial class CallListWindow : Window
    //{
    //    // Access to the BL
    //    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    //    // DependencyProperty for the Call list
    //    public IEnumerable<BO.CallInList> CallList
    //    {
    //        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
    //        set { SetValue(CallListProperty, value); }
    //    }
    //    public static readonly DependencyProperty CallListProperty =
    //        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow));

    //    // DependencyProperty for the selected call
    //    public BO.CallInList? SelectedCall { get; set; }

    //    // DependencyProperty for selected sort type
    //    public string SelectedSortBy { get; set; } = "OpenTime";

    //    // DependencyProperty for selected filter type
    //    public string SelectedFilterBy { get; set; } = "CallType";

    //    // DependencyProperty for filter value
    //    public string FilterValue { get; set; } = string.Empty;

    //    // DependencyProperty for filter value visibility
    //    public bool IsFilterTextBoxVisible { get; set; } = true;

    //    private void cmbFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        // קבלת הערך הנבחר מתוך ה-ComboBox
    //        var selectedItem = ((ComboBox)sender).SelectedItem as ComboBoxItem;

    //        if (selectedItem != null)
    //        {
    //            // אם הפריט הנבחר הוא ComboBoxItem, קבלת ה-Tag שלו
    //            SelectedFilterBy = selectedItem.Tag.ToString();
    //        }
    //        else
    //        {
    //            // אם נבחר ערך אחר, ניתן לשנות את הלוגיקה בהתאם, למשל אם מדובר במחרוזת:
    //            SelectedFilterBy = ((ComboBox)sender).SelectedItem.ToString();
    //        }

    //        // קריאה לפונקציה שמספקת את רשימת הקריאות לפי הסינון
    //        queryCallList();
    //    }

    //    private void cmbSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        // קבלת הערך שנבחר מה-ComboBox (נניח אתה רוצה את ה-Tag)
    //        var selectedItem = ((ComboBox)sender).SelectedItem as ComboBoxItem;

    //        if (selectedItem != null)
    //        {
    //            // עדכון ערך המשתנה SelectedSortBy מה-Tag של הפריט
    //            SelectedSortBy = selectedItem.Tag.ToString();
    //        }

    //        // קריאה לפונקציה שמספקת את רשימת הקריאות לפי הסינון
    //        queryCallList();
    //    }


    //    private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    //    {
    //        // קבלת הקריאה שנבחרה
    //        var selectedCall = SelectedCall; // או אם צריך: (DataGrid)sender).SelectedItem;

    //        // פעולה שתתבצע כאשר יש לחיצה כפולה על שורה
    //        if (selectedCall != null)
    //        {
    //            // לדוגמה, פתיחת פרטי הקריאה או פעולה אחרת
    //            // תוכל להוסיף את הלוגיקה המתאימה כאן
    //        }
    //    }

    //    // DependencyProperty for combo box visibility
    //    private void queryCallList()
    //    {
    //        if (SelectedFilterBy == "CallType" && !string.IsNullOrEmpty(FilterValue))
    //        {
    //            // המרת המחרוזת לערך Enum מתאים
    //            BO.CallInListSort? sortType = Enum.TryParse<BO.CallInListSort>(SelectedSortBy, out var result) ? result : (BO.CallInListSort?)null;

    //            CallList = s_bl?.Call.GetListOfCalls(
    //                BO.CallInListSort.CallType, FilterValue, sortType);
    //        }
    //        else if (SelectedFilterBy == "CallStatus" && !string.IsNullOrEmpty(FilterValue))
    //        {
    //            // המרת המחרוזת לערך Enum מתאים
    //            BO.CallInListSort? sortType = Enum.TryParse<BO.CallInListSort>(SelectedSortBy, out var result) ? result : (BO.CallInListSort?)null;

    //            CallList = s_bl?.Call.GetListOfCalls(
    //                BO.CallInListSort.CallInListStatus, FilterValue, sortType);
    //        }
    //        else
    //        {
    //            // המרת המחרוזת לערך Enum מתאים
    //            BO.CallInListSort? sortType = Enum.TryParse<BO.CallInListSort>(SelectedSortBy, out var result) ? result : (BO.CallInListSort?)null;

    //            CallList = s_bl?.Call.GetListOfCalls(sortType);
    //        }
    //    }



    //    // Function to be called whenever the filter or sort option changes
    //    private void CallListObserver()
    //    {
    //        queryCallList();
    //    }

    //    // Window Loaded event handler
    //    private void Window_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        s_bl.Call.AddObserver(CallListObserver);
    //    }

    //    // Window Closed event handler
    //    private void Window_Closed(object sender, EventArgs e)
    //    {
    //        s_bl.Call.RemoveObserver(CallListObserver);
    //    }

    //    // Constructor for the call list window
    //    public CallListWindow()
    //    {
    //        InitializeComponent();
    //        queryCallList();
    //        Loaded += Window_Loaded;
    //        Closed += Window_Closed;
    //    }
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // DependencyProperty for the Call list
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow));

        // DependencyProperty for the selected call
        public BO.CallInList? SelectedCall { get; set; }

        // Properties for sorting, filtering and values
        public static readonly DependencyProperty SelectedSortByProperty =
            DependencyProperty.Register("SelectedSortBy", typeof(string), typeof(CallListWindow),
            new PropertyMetadata("OpenTime"));

        public static readonly DependencyProperty SelectedFilterByProperty =
            DependencyProperty.Register("SelectedFilterBy", typeof(string), typeof(CallListWindow),
            new PropertyMetadata("CallType"));

        public static readonly DependencyProperty FilterValueOptionsProperty =
            DependencyProperty.Register("FilterValueOptions", typeof(IEnumerable<string>),
            typeof(CallListWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedFilterValueProperty =
            DependencyProperty.Register("SelectedFilterValue", typeof(string),
            typeof(CallListWindow), new PropertyMetadata(null));

        public string SelectedSortBy
        {
            get { return (string)GetValue(SelectedSortByProperty); }
            set { SetValue(SelectedSortByProperty, value); }
        }

        public string SelectedFilterBy
        {
            get { return (string)GetValue(SelectedFilterByProperty); }
            set { SetValue(SelectedFilterByProperty, value); }
        }

        public IEnumerable<string> FilterValueOptions
        {
            get { return (IEnumerable<string>)GetValue(FilterValueOptionsProperty); }
            set { SetValue(FilterValueOptionsProperty, value); }
        }

        public string SelectedFilterValue
        {
            get { return (string)GetValue(SelectedFilterValueProperty); }
            set { SetValue(SelectedFilterValueProperty, value); }
        }

        private void cmbFilterValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // גישה ישירה לפריט הנבחר מבלי להמיר אותו ל-ComboBoxItem
            var selectedItem = ((ComboBox)sender).SelectedItem as string;
            if (selectedItem == null) return;

            SelectedFilterBy = selectedItem;

            // עדכון אפשרויות הפילטר בהתאם לבחירה
            FilterValueOptions = SelectedFilterBy switch
            {
                "CallType" => Enum.GetNames(typeof(BO.CallType)),
                "CallInListStatus" => Enum.GetNames(typeof(BO.CallInListStatus)),
                _ => Array.Empty<string>()
            };

            queryCallList();
        }

        private void cmbSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // גישה ישירה לפריט הנבחר מבלי להמיר אותו ל-ComboBoxItem
            var selectedItem = ((ComboBox)sender).SelectedItem as string;
            if (selectedItem == null) return;

            SelectedSortBy = selectedItem;
            queryCallList();
        }


        private void queryCallList()
        {
            // המרת ערך המיון ל-enum
            BO.CallInListSort? sortType = null;
            if (Enum.TryParse<BO.CallInListSort>(SelectedSortBy, out var parsedSortType))
            {
                sortType = parsedSortType;
            }

            // אם אין ערך פילטר נבחר
            if (string.IsNullOrEmpty(SelectedFilterValue))
            {
                CallList = s_bl?.Call.GetListOfCalls(sortType: sortType);
                return;
            }

            // בחירת סוג הפילטר והפעלתו
            if (SelectedFilterBy == "CallType")
            {
                CallList = s_bl?.Call.GetListOfCalls(BO.CallInListSort.CallType, SelectedFilterValue, sortType);
            }
            else if (SelectedFilterBy == "CallInListStatus")
            {
                CallList = s_bl?.Call.GetListOfCalls(BO.CallInListSort.CallInListStatus, SelectedFilterValue, sortType);
            }
        }

        private void CallListObserver()
        {
            queryCallList();
        }

        public CallListWindow()
        {
            InitializeComponent();
            queryCallList();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallListObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        ///-----------------------------------------BUTTONS------------------------------------

        /// <summary>
        /// Delete function for call
        /// </summary>
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

                        MessageBox.Show($"Call with ID {callId} has been successfully deleted.",
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
        /// Add function for new call
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        /// <summary>
        /// Function to handle sorting selection changes
        /// </summary>
        private void ComboBoxSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSortBy = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag.ToString();
            queryCallList();
        }

        /// <summary>
        /// Function to handle filter selection changes
        /// </summary>
        private void cmbFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((ComboBox)sender).SelectedItem as string;
            if (selectedItem == null) return;

            SelectedFilterBy = selectedItem;

            // עדכון אפשרויות הפילטר בהתאם לבחירה
            FilterValueOptions = SelectedFilterBy switch
            {
                "CallType" => Enum.GetNames(typeof(BO.CallType)),
                "CallInListStatus" => Enum.GetNames(typeof(BO.CallInListStatus)),
                _ => Array.Empty<string>()
            };

            // עדכון הקומבובוקס הימני עם הערכים החדשים
            cmbFilterValue.ItemsSource = FilterValueOptions;

            // מאפס את הבחירה הנוכחית
            cmbFilterValue.SelectedIndex = -1;

            // ביצוע השאילתה מחדש
            queryCallList();
        }


        /// <summary>
        /// Function to handle double-click on a call in the list
        /// </summary>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new CallWindow(SelectedCall.CallId).Show();
        }
    }
}

