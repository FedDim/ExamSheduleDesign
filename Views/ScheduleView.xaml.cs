using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    /// <summary>
    /// Логика взаимодействия для ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl
    {
        public ScheduleView()
        {
            InitializeComponent();
            this.DataContext = new ScheduleViewModel(App.DataService);
        }
    }
}
