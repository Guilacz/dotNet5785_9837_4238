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
using System.Windows.Threading;

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        //---------------------Properties / Dependency properties---------------------------


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

        //instead of taking the enum, we added an enumerable of strings
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
        public IEnumerable<string> FilterByOptions { get; } = new[] { "CallType", "CallInListStatus" }; 

        public IEnumerable<string> SortByOptions { get; } = new[] { "CallId", "OpenTime", "LastName" }; 

 

        private DateTime? _startDate; 
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



        //---------------------FUNCTIONS---------------------------





        /// <summary>
        /// FILTER WITH VALUE FUNCTION
        /// </summary>
        private void UpdateFilterValueOptions()
        {
            FilterValueOptions = SelectedFilterBy switch
            {
                "CallType" => Enum.GetNames(typeof(BO.CallType)),
                "CallInListStatus" => Enum.GetNames(typeof(BO.CallInListStatus)),
                "Date" => Array.Empty<string>(),
                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        /// query function of the screen
        /// </summary>
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


        private volatile DispatcherOperation? _callListOperation = null;

        /// <summary>
        /// observer of the page
        /// </summary>
        private void CallListObserver()
        {
            if (_callListOperation is null || _callListOperation.Status == DispatcherOperationStatus.Completed)
            {
                _callListOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryCallList();
                });
            }
        }


       

        /// <summary>
        /// Event handler for when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallListObserver);
        }

        /// <summary>
        /// Event handler for when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        /// <summary>
        /// constructor of the window
        /// </summary>
        public CallListWindow(BO.CallInListStatus? status)
        {
            InitializeComponent();
            

            UpdateFilterValueOptions();

            

            if (status != null)
            {
                // Définir automatiquement le filtre par statut
                SelectedFilterBy = "CallInListStatus";
                SelectedFilterValue = status.ToString();
            }
            QueryCallList();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }



        //-------------------BUTTONS-------------------------

        /// <summary>
        /// delete button
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

        /// <summary>
        /// add button
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        private void ComboBoxSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSortBy = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag.ToString();
            QueryCallList();
        }

        //Event for when we click on the grid
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                new CallWindow(SelectedCall.CallId).Show();
            }
        }

    }
}
