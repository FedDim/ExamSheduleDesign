using System;

namespace ExamSheduleDesign.Services.DTO
{
    public class BufferExamDto
    {
        public int Id { get; set; } // не используется при восстановлении
        public DateTime Date { get; set; }
        public string Time { get; set; } = "";
        public string Type { get; set; } = "";
        public int Teacher1Id { get; set; }
        public int? Teacher2Id { get; set; }
        public int DisciplineId { get; set; }
        public int GroupId { get; set; }
        public string Classroom { get; set; } = "";
        public bool IsSelected { get; set; }
    }
}