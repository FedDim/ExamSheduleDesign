using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ExamSheduleDesign.Models
{
    public partial class Exam : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Экзамен" или "Консультация"
        public Teacher Teacher1 { get; set; }
        public Teacher Teacher2 { get; set; }
        public Discipline Discipline { get; set; }
        public Group Group { get; set; }
        public string Classroom { get; set; } = string.Empty;
    }
}