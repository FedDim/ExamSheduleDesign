using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class MainView : UserControl
    {
        private readonly MainViewModel _viewModel;

        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        // Обработчики кнопок буфера теперь вызывают методы ViewModel
        private void SaveBufferBtn_Click(object sender, System.Windows.RoutedEventArgs e) => _viewModel.SaveBuffer();
        private void LoadBufferBtn_Click(object sender, System.Windows.RoutedEventArgs e) => _viewModel.LoadBuffer();
        private void ClearBufferBtn_Click(object sender, System.Windows.RoutedEventArgs e) => _viewModel.ClearBuffer();
    }
}