using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class AddDataView : UserControl
    {
        private readonly AddDataViewModel _viewModel;

        public AddDataView(AddDataViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadDataAsync();
        }
    }
}