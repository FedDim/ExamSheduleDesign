using ExamSheduleDesign.Helpers;
using ExamSheduleDesign.ViewModels;
using MahApps.Metro.Controls;
using System.Windows;

namespace ExamSheduleDesign
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            WindowAcrylicHelper.EnableAcrylicBlur(this, 0x19FFFFFF);

            // Подписываемся на изменение состояния окна
            this.StateChanged += MainWindow_StateChanged;
            // Устанавливаем начальное скругление
            UpdateCornerRadius();
        }

        private void MainWindow_StateChanged(object sender, System.EventArgs e)
        {
            UpdateCornerRadius();
        }

        private void UpdateCornerRadius()
        {
            if (WindowState == WindowState.Maximized)
            {
                // При максимизации убираем скругления
                ContentBorder.CornerRadius = new CornerRadius(0);
            }
            else
            {
                // В обычном режиме возвращаем скругления (только левая верхняя и левая нижняя)
                ContentBorder.CornerRadius = new CornerRadius(12, 0, 0, 12);
            }
        }
    }
}