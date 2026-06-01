using ExamSheduleDesign.Services;
using ExamSheduleDesign.ViewModels;
using MahApps.Metro.Controls;
using System.Windows.Input;

namespace ExamSheduleDesign
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            App.NotificationService = new NotificationService(NotificationToast);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.IsDevMode = !vm.IsDevMode;
                }
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}