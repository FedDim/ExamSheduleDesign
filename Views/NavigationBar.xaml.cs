using ExamSheduleDesign.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class NavigationBar : UserControl
    {
        public NavigationBar()
        {
            InitializeComponent();

            // Подписываемся на события выбора всех трёх списков
            NavListBoxMain.SelectionChanged += OnSelectionChanged;
            NavListBoxData.SelectionChanged += OnSelectionChanged;
            NavListBoxService.SelectionChanged += OnSelectionChanged;

            Loaded += (s, e) =>
            {
                if (NavListBoxMain.Items.Count > 0)
                    NavListBoxMain.SelectedItem = NavListBoxMain.Items[0];
            };
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceListBox = sender as ListBox;
            if (sourceListBox?.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag is string tag)
            {

                // Синхронизация: снимаем выделение в других списках
                if (sourceListBox != NavListBoxMain) NavListBoxMain.SelectedItem = null;
                if (sourceListBox != NavListBoxData) NavListBoxData.SelectedItem = null;
                if (sourceListBox != NavListBoxService) NavListBoxService.SelectedItem = null;

                // Выполняем команду навигации, если ViewModel доступна
                if (Application.Current.MainWindow.DataContext is MainWindowViewModel vm)
                {
                    switch (tag)
                    {
                        case "Main": vm.NavigateToMainCommand.Execute(null); break;
                        case "Schedule": vm.NavigateToScheduleCommand.Execute(null); break;
                        case "Add": vm.NavigateToAddCommand.Execute(null); break;
                        case "Edit": vm.NavigateToEditCommand.Execute(null); break;
                        case "Dev": vm.NavigateToDevCommand.Execute(null); break;
                    }
                }
            }
        }
    }
}