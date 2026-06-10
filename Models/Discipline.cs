using CommunityToolkit.Mvvm.ComponentModel;

namespace ExamSheduleDesign.Models
{
    public partial class Discipline : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;

        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShortName12 { get; set; } = string.Empty;
        public string ShortName9 { get; set; } = string.Empty;
        public string ShortName5 { get; set; } = string.Empty;
    }
}
