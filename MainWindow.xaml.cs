using ExamSheduleDesign.ViewModels;
using MahApps.Metro.Controls;

namespace ExamSheduleDesign
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
    }
}