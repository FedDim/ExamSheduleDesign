using ExamSheduleDesign.Models;
using System.Collections.Generic;

namespace ExamSheduleDesign.Services
{
    public interface IDataService
    {
        List<Teacher> GetTeachers();
        List<Discipline> GetDisciplines();
        List<Group> GetGroups();
        void AddExam(Exam exam);
        List<Exam> GetExams();
    }
}
