using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class EditDataView : UserControl
    {
        private readonly EditDataViewModel _viewModel;

        public EditDataView(EditDataViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadDataAsync();
        }
    }
}