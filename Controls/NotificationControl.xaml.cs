using MahApps.Metro.IconPacks; // добавить ссылку на пакет иконок
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ExamSheduleDesign.Controls
{
    public enum NotificationType
    {
        Information,
        Error,
        Warning,
        Success
    }

    public partial class NotificationControl : UserControl
    {
        private Storyboard _showStoryboard;
        private Storyboard _hideStoryboard;

        public NotificationControl()
        {
            InitializeComponent();
            this.RenderTransform = new TranslateTransform();
            _showStoryboard = (Storyboard)FindResource("ShowAnimation");
            _hideStoryboard = (Storyboard)FindResource("HideAnimation");
            _hideStoryboard.Completed += (s, e) => Visibility = Visibility.Collapsed;
        }

        public void Show(string message, NotificationType type = NotificationType.Information)
        {
            MessageText.Text = message;

            Brush colorBrush;
            PackIconModernKind iconKind;
            switch (type)
            {
                case NotificationType.Error:
                    colorBrush = (Brush)Application.Current.Resources["NotificationErrorBrush"];
                    iconKind = PackIconModernKind.Cancel;
                    break;
                case NotificationType.Warning:
                    colorBrush = (Brush)Application.Current.Resources["NotificationWarningBrush"];
                    iconKind = PackIconModernKind.Alert;
                    break;
                case NotificationType.Success:
                    colorBrush = (Brush)Application.Current.Resources["NotificationSuccessBrush"];
                    iconKind = PackIconModernKind.Checkmark;
                    break;
                default:
                    colorBrush = (Brush)Application.Current.Resources["AccentBrush"];
                    iconKind = PackIconModernKind.Information;
                    break;
            }
            BorderRoot.BorderBrush = colorBrush;
            Icon.Foreground = colorBrush;
            Icon.Kind = iconKind;

            Visibility = Visibility.Visible;
            _showStoryboard.Begin(this);

            int charCount = message.Length;
            int seconds = Math.Max(2, 2 + charCount / 20);
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(seconds)
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();
                _hideStoryboard.Begin(this);
            };
            timer.Start();
        }
    }
}