using ExamSheduleDesign.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class NavigationBar : UserControl
    {
        private bool _isUpdating;
        private readonly INavigationService _navigationService;

        public NavigationBar()
        {
            InitializeComponent();
            _navigationService = App.Services.GetRequiredService<INavigationService>();

            NavListBoxMain.SelectionChanged += OnSelectionChanged;
            NavListBoxData.SelectionChanged += OnSelectionChanged;
            NavListBoxService.SelectionChanged += OnSelectionChanged;
            NavListBoxSystem.SelectionChanged += OnSelectionChanged;

            // Подписываемся на изменения навигации, чтобы обновлять выделение при программной навигации
            _navigationService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(INavigationService.CurrentPage))
                {
                    Dispatcher.Invoke(() => SetSelectedItemByTag(GetCurrentTag()));
                }
            };

            Loaded += async (s, e) =>
            {
                await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                SetSelectedItemByTag(GetCurrentTag());
            };
        }

        private string GetCurrentTag()
        {
            if (_navigationService.CurrentPage is MainView) return "Main";
            if (_navigationService.CurrentPage is ScheduleView) return "Schedule";
            if (_navigationService.CurrentPage is AddDataView) return "Add";
            if (_navigationService.CurrentPage is EditDataView) return "Edit";
            if (_navigationService.CurrentPage is DevView) return "Dev";
            if (_navigationService.CurrentPage is SettingsView) return "Settings";
            return "Main";
        }

        private void SetSelectedItemByTag(string tag)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
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
                    if (sourceListBox != NavListBoxMain) NavListBoxMain.SelectedItem = null;
                    if (sourceListBox != NavListBoxData) NavListBoxData.SelectedItem = null;
                    if (sourceListBox != NavListBoxService) NavListBoxService.SelectedItem = null;
                    if (sourceListBox != NavListBoxSystem) NavListBoxSystem.SelectedItem = null;

                    switch (tag)
                    {
                        case "Main": _navigationService.NavigateToMain(); break;
                        case "Schedule": _navigationService.NavigateToSchedule(); break;
                        case "Add": _navigationService.NavigateToAdd(); break;
                        case "Edit": _navigationService.NavigateToEdit(); break;
                        case "Dev": _navigationService.NavigateToDev(); break;
                        case "Settings": _navigationService.NavigateToSettings(); break;
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