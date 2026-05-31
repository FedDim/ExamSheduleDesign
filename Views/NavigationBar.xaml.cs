using ExamSheduleDesign.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class NavigationBar : UserControl
    {
        private bool _isUpdating; // Защита от рекурсии

        public NavigationBar()
        {
            InitializeComponent();

            NavListBoxMain.SelectionChanged += OnSelectionChanged;
            NavListBoxData.SelectionChanged += OnSelectionChanged;
            NavListBoxService.SelectionChanged += OnSelectionChanged;

            Loaded += async (s, e) =>
            {
                // Даём время MainWindow полностью инициализировать DataContext
                await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                if (NavListBoxMain.Items.Count > 0)
                {
                    // Временно отключаем обработчик, чтобы не вызывать навигацию
                    NavListBoxMain.SelectionChanged -= OnSelectionChanged;
                    NavListBoxMain.SelectedItem = NavListBoxMain.Items[0];
                    NavListBoxMain.SelectionChanged += OnSelectionChanged;
                }
            };
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                var sourceListBox = sender as ListBox;
                if (sourceListBox?.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag is string tag)
                {
                    // Синхронизация выделения
                    if (sourceListBox != NavListBoxMain) NavListBoxMain.SelectedItem = null;
                    if (sourceListBox != NavListBoxData) NavListBoxData.SelectedItem = null;
                    if (sourceListBox != NavListBoxService) NavListBoxService.SelectedItem = null;

                    if (Application.Current.MainWindow.DataContext is MainWindowViewModel vm)
                    {
                        switch (tag)
                        {
                            case "Main": vm.NavigateToMainCommand?.Execute(null); break;
                            case "Schedule": vm.NavigateToScheduleCommand?.Execute(null); break;
                            case "Add": vm.NavigateToAddCommand?.Execute(null); break;
                            case "Edit": vm.NavigateToEditCommand?.Execute(null); break;
                            case "Dev": vm.NavigateToDevCommand?.Execute(null); break;
                        }
                    }
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}