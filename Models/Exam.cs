using System;

namespace ExamSheduleDesign.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; } // "Экзамен" или "Консультация"
        public Teacher Teacher1 { get; set; }
        public Teacher Teacher2 { get; set; }
        public Discipline Discipline { get; set; }
        public Group Group { get; set; }
        public string Classroom { get; set; }
    }
}
