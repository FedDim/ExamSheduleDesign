using System.Windows;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.MainViewModel(App.DataService);
        }

        // Обработчики для левых кнопок (пока вызывают методы ViewModel)
        private void SaveBufferBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).SaveBuffer();
        private void LoadBufferBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).LoadBuffer();
        private void ClearBufferBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).ClearBuffer();
    }
}
