using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services;
using ExamSheduleDesign.ViewModels;
using MahApps.Metro.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class EditExamDialog : MetroWindow
    {
        public EditExamDialog(Exam exam, IDataService dataService)
        {
            InitializeComponent();
            var viewModel = new EditExamDialogViewModel(exam, dataService, this);
            DataContext = viewModel;
        }
    }
}