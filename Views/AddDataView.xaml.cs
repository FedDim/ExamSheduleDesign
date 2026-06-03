using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class AddDataView : UserControl
    {
        public AddDataView(AddDataViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}