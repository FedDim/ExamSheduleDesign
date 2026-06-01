using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ExamSheduleDesign.Controls
{
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

        public void Show(string message)
        {
            MessageText.Text = message;
            Visibility = Visibility.Visible;
            _showStoryboard.Begin(this);
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                _hideStoryboard.Begin(this);
            };
            timer.Start();
        }
    }
}