using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class EditDataView : UserControl
    {
        public EditDataView(EditDataViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}