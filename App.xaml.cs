using ExamSheduleDesign.Services;
using System.Windows;

namespace ExamSheduleDesign
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IDataService DataService { get; } = new MockDataService();
        public static INotificationService NotificationService { get; set; }
    }
}
