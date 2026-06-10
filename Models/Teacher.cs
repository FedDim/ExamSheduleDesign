using CommunityToolkit.Mvvm.ComponentModel;

namespace ExamSheduleDesign.Models
{
    public partial class Teacher : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
        public int AcademicBuilding { get; set; }
    }
}