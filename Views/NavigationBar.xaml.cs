using ExamSheduleDesign.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class NavigationBar : UserControl
    {
        private bool _isUpdating;

        public NavigationBar()
        {
            InitializeComponent();

            NavListBoxMain.SelectionChanged += OnSelectionChanged;
            NavListBoxData.SelectionChanged += OnSelectionChanged;
            NavListBoxService.SelectionChanged += OnSelectionChanged;
            NavListBoxSystem.SelectionChanged += OnSelectionChanged;

            Loaded += async (s, e) =>
            {
                await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                if (Application.Current.MainWindow.DataContext is MainWindowViewModel vm)
                {
                    // Подписываемся на изменение CurrentPageTag
                    vm.PropertyChanged += (_, args) =>
                    {
                        if (args.PropertyName == nameof(MainWindowViewModel.CurrentPageTag))
                            SetSelectedItemByTag(vm.CurrentPageTag);
                    };
                    SetSelectedItemByTag(vm.CurrentPageTag);
                }
            };
        }

        private void SetSelectedItemByTag(string tag)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                // Сброс выделения во всех списках
                NavListBoxMain.SelectedItem = null;
                NavListBoxData.SelectedItem = null;
                NavListBoxService.SelectedItem = null;
                NavListBoxSystem.SelectedItem = null;

                switch (tag)
                {
                    case "Main": NavListBoxMain.SelectedItem = FindItemByTag(NavListBoxMain, tag); break;
                    case "Schedule": NavListBoxMain.SelectedItem = FindItemByTag(NavListBoxMain, tag); break;
                    case "Add": NavListBoxData.SelectedItem = FindItemByTag(NavListBoxData, tag); break;
                    case "Edit": NavListBoxData.SelectedItem = FindItemByTag(NavListBoxData, tag); break;
                    case "Dev": NavListBoxService.SelectedItem = FindItemByTag(NavListBoxService, tag); break;
                    case "Settings": NavListBoxSystem.SelectedItem = FindItemByTag(NavListBoxSystem, tag); break;
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private ListBoxItem FindItemByTag(ListBox listBox, string tag)
        {
            foreach (ListBoxItem item in listBox.Items)
                if (item.Tag as string == tag) return item;
            return null;
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
                    // Синхронизация: снимаем выделение в других списках
                    if (sourceListBox != NavListBoxMain) NavListBoxMain.SelectedItem = null;
                    if (sourceListBox != NavListBoxData) NavListBoxData.SelectedItem = null;
                    if (sourceListBox != NavListBoxService) NavListBoxService.SelectedItem = null;
                    if (sourceListBox != NavListBoxSystem) NavListBoxSystem.SelectedItem = null;

                    if (Application.Current.MainWindow.DataContext is MainWindowViewModel vm)
                    {
                        switch (tag)
                        {
                            case "Main": vm.NavigateToMainCommand.Execute(null); break;
                            case "Schedule": vm.NavigateToScheduleCommand.Execute(null); break;
                            case "Add": vm.NavigateToAddCommand.Execute(null); break;
                            case "Edit": vm.NavigateToEditCommand.Execute(null); break;
                            case "Dev": vm.NavigateToDevCommand.Execute(null); break;
                            case "Settings": vm.NavigateToSettingsCommand.Execute(null); break;
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