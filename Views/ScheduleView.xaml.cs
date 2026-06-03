using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class ScheduleView : UserControl
    {
        private readonly ScheduleViewModel _viewModel;

        public ScheduleView(ScheduleViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadExamsAsync();
        }
    }
}