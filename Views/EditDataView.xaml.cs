using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    /// <summary>
    /// Логика взаимодействия для EditDataView.xaml
    /// </summary>
    public partial class EditDataView : UserControl
    {
        public EditDataView()
        {
            InitializeComponent();
            this.DataContext = new EditDataViewModel(App.DataService);
        }
    }
}
