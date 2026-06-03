using ExamSheduleDesign.Services;
using ExamSheduleDesign.ViewModels;
using MahApps.Metro.Controls;
using System.Windows.Input;

namespace ExamSheduleDesign
{
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Передаём контрол уведомлений в сервис
            notificationService.Initialize(NotificationToast);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _viewModel.IsDevMode = !_viewModel.IsDevMode;
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}