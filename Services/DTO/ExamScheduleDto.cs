namespace ExamSheduleDesign.Services.DTO
{
    public class ExamScheduleDto
    {
        public int Id { get; set; }
        public int Teacher1Id { get; set; }
        public int? Teacher2Id { get; set; }
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public string Classroom { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ExamDate { get; set; } = "";
        public string ExamTime { get; set; } = "";
        public string ExamType { get; set; } = "";
    }
}