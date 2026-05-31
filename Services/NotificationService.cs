using ExamSheduleDesign.Controls;
using System.Windows;

namespace ExamSheduleDesign.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationControl _notificationControl;

        public NotificationService(NotificationControl notificationControl)
        {
            _notificationControl = notificationControl;
        }

        public void Show(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notificationControl.Show(message);
            });
        }
    }
}