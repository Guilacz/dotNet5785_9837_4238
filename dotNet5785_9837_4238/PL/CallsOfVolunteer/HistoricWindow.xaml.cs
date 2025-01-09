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

namespace PL.CallsOfVolunteer
{
    /// <summary>
    /// Interaction logic for HistoricWindow.xaml
    /// </summary>
    public partial class HistoricWindow : Window
    {

        //access to the BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        //call list 
        private IEnumerable<BO.ClosedCallInList> _allCalls;
        private int volunteerId;




        public HistoricWindow(int id)
        {
            InitializeComponent();
            volunteerId = id;
        }

        private void LoadData()
        {
            _allCalls = s_bl.Call.GetListOfClosedCall(volunteerId);

        }
    }
}
