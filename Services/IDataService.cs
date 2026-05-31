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
        void RemoveExams(IEnumerable<Exam> exams);
        void AddTeacher(Teacher teacher);
        void AddDiscipline(Discipline discipline);
        void AddGroup(Group group);
        void UpdateTeachers(IEnumerable<Teacher> updatedTeachers);
        void UpdateDisciplines(IEnumerable<Discipline> updatedDisciplines);
        void UpdateGroups(IEnumerable<Group> updatedGroups);
        void GenerateExams(int count);
        void ClearGeneratedExams();
        int GetGeneratedExamsCount();
        List<Exam> GetExams();
    }
}
