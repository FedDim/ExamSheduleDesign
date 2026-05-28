using ExamSheduleDesign.Helpers;
using ExamSheduleDesign.ViewModels;
using MahApps.Metro.Controls;

namespace ExamSheduleDesign
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            // Включаем акрил
            WindowAcrylicHelper.EnableAcrylicBlur(this, 0x19FFFFFF);
        }
    }
}
