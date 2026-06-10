using CommunityToolkit.Mvvm.ComponentModel;

namespace ExamSheduleDesign.Models
{
    public partial class Group : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
