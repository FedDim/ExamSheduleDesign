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
        private void AddTeacherBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).AddTeacher();
        private void AddDisciplineBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).AddDiscipline();
        private void AddGroupBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).AddGroup();
        private void EditTeachersBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).EditTeachers();
        private void EditDisciplinesBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).EditDisciplines();
        private void EditGroupsBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).EditGroups();
        private void SaveBufferBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).SaveBuffer();
        private void LoadBufferBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).LoadBuffer();
        private void ClearBufferBtn_Click(object sender, RoutedEventArgs e) => ((ViewModels.MainViewModel)DataContext).ClearBuffer();
    }
}
