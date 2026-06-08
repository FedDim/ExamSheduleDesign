using ExamSheduleDesign.Controls;
using System.Windows;

namespace ExamSheduleDesign.Services
{
    public class NotificationService : INotificationService
    {
        private NotificationControl _notificationControl;

        public void Initialize(NotificationControl notificationControl)
        {
            _notificationControl = notificationControl;
        }

        public void Show(string message, NotificationType type = NotificationType.Information)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_notificationControl != null)
                    _notificationControl.Show(message, type);
                else
                    MessageBox.Show(message);
            });
        }
    }
}