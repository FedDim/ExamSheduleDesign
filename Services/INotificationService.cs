using ExamSheduleDesign.Controls;

namespace ExamSheduleDesign.Services
{
    public interface INotificationService
    {
        void Initialize(NotificationControl notificationControl);
        void Show(string message, NotificationType type = NotificationType.Information);
    }
}