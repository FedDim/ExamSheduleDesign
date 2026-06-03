namespace ExamSheduleDesign.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
        public int AcademicBuilding { get; set; }
    }
}