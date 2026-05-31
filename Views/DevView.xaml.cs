using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class DevView : UserControl
    {
        public DevView()
        {
            InitializeComponent();
            this.DataContext = new DevViewModel(App.DataService);
        }
    }
}