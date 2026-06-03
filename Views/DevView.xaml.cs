using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class DevView : UserControl
    {
        private readonly DevViewModel _viewModel;

        public DevView(DevViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadDataAsync();
        }
    }
}