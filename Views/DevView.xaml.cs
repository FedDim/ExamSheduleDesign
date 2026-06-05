using ExamSheduleDesign.ViewModels;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class DevView : UserControl
    {
        private readonly DevViewModel _viewModel;

        public DevView(DevViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += (s, e) =>
            {
                if (DataContext is DevViewModel vm)
                    PasswordBox.Password = vm.Password;
            };
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is DevViewModel vm)
                vm.Password = PasswordBox.Password;
        }
    }
}