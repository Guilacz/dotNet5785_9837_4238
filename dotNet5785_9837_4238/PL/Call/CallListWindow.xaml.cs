using BO;
using PL.Volunteer;
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

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Properties for filtering and sorting
        private string _selectedFilterBy;
        public string SelectedFilterBy
        {
            get => _selectedFilterBy;
            set
            {
                if (_selectedFilterBy != value)
                {
                    _selectedFilterBy = value;
                    OnPropertyChanged(nameof(SelectedFilterBy));
                    UpdateFilterValueOptions();
                }
            }
        }


        // DependencyProperty עבור הקריאה הנבחרת
        public BO.CallInList? SelectedCall
        {
            get { return (BO.CallInList?)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(BO.CallInList), typeof(CallListWindow));
        // DependencyProperty עבור רשימת הקריאות
        public IEnumerable<BO.CallInList> CallList
            {
                get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
                set { SetValue(CallListProperty, value); }
            }
            public static readonly DependencyProperty CallListProperty =
                DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow));
        
        private string _selectedFilterValue;
        public string SelectedFilterValue
        {
            get => _selectedFilterValue;
            set
            {
                if (_selectedFilterValue != value)
                {
                    _selectedFilterValue = value;
                    OnPropertyChanged(nameof(SelectedFilterValue));
                    QueryCallList();
                }
            }
        }

        private string _selectedSortBy;
        public string SelectedSortBy
        {
            get => _selectedSortBy;
            set
            {
                if (_selectedSortBy != value)
                {
                    _selectedSortBy = value;
                    OnPropertyChanged(nameof(SelectedSortBy));
                    QueryCallList();
                }
            }
        }

        private IEnumerable<string> _filterValueOptions;
        public IEnumerable<string> FilterValueOptions
        {
            get => _filterValueOptions;
            private set
            {
                _filterValueOptions = value;
                OnPropertyChanged(nameof(FilterValueOptions));
            }
        }
        public IEnumerable<string> FilterByOptions { get; } = new[] { "CallType", "CallInListStatus" }; // לא כוללים תאריך

        public IEnumerable<string> SortByOptions { get; } = new[] { "CallId", "OpenTime", "LastName" }; // אפשרויות למיון

 

        private DateTime? _startDate; // תאריך התחלה
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                    QueryCallList();
                }
            }
        }

        private DateTime? _endDate; // תאריך סיום
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                    QueryCallList();
                }
            }
        }

        private void UpdateFilterValueOptions()
        {
            FilterValueOptions = SelectedFilterBy switch
            {
                "CallType" => Enum.GetNames(typeof(BO.CallType)),
                "CallInListStatus" => Enum.GetNames(typeof(BO.CallInListStatus)),
                "Date" => Array.Empty<string>(), // סינון לפי תאריך לא מצריך ערכים
                _ => Array.Empty<string>()
            };
        }

        private void QueryCallList()
        {
            BO.CallInListSort? sortType = null;

            if (Enum.TryParse<BO.CallInListSort>(SelectedSortBy, out var parsedSortType))
            {
                sortType = parsedSortType;
            }

            if (SelectedFilterBy == "Date" && StartDate.HasValue && EndDate.HasValue)
            {
                CallList = s_bl?.Call.GetListOfCalls(
                    filterType: BO.CallInListSort.OpenTime,
                    filterValue: new { StartDate = StartDate.Value, EndDate = EndDate.Value },
                    sortType: sortType
                );
                return;
            }


            if (string.IsNullOrEmpty(SelectedFilterValue))
            {
                CallList = s_bl?.Call.GetListOfCalls(sortType: sortType);
                return;
            }

            CallList = SelectedFilterBy switch
            {
                "CallType" => s_bl?.Call.GetListOfCalls(filterType: BO.CallInListSort.CallType, filterValue: Enum.Parse<BO.CallType>(SelectedFilterValue), sortType: sortType),
                "CallInListStatus" => s_bl?.Call.GetListOfCalls(filterType: BO.CallInListSort.CallInListStatus, filterValue: Enum.Parse<BO.CallInListStatus>(SelectedFilterValue), sortType: sortType),
                _ => CallList
            };

        }
        // Function to be called whenever the filter or sort option changes
        private void CallListObserver()
        {
            QueryCallList();
        }

        // Window Loaded event handler
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallListObserver);
        }

        // Window Closed event handler
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        public CallListWindow()
        {
            InitializeComponent();
            UpdateFilterValueOptions();
            QueryCallList();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
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
                        QueryCallList();
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        private void ComboBoxSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSortBy = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag.ToString();
            QueryCallList();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                new CallWindow(SelectedCall.CallId).Show();
            }
        }

    }
}
