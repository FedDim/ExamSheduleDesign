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
            NavListBox.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavListBox.SelectedItem is ListBoxItem item && item.Tag is string tag)
            {
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
