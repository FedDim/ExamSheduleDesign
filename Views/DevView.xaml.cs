using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class DevView : UserControl
    {
        public DevView(DevViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}