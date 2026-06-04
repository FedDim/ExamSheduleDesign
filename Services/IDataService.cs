using ExamSheduleDesign.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public interface IDataService
    {
        Task<List<Teacher>> GetTeachersAsync();
        Task<List<Discipline>> GetDisciplinesAsync();
        Task<List<Group>> GetGroupsAsync();
        Task<List<Exam>> GetExamsAsync();
        Task AddExamAsync(Exam exam);
        Task RemoveExamsAsync(IEnumerable<Exam> exams);
        Task AddTeacherAsync(Teacher teacher);
        Task AddDisciplineAsync(Discipline discipline);
        Task AddGroupAsync(Group group);
        Task UpdateTeachersAsync(IEnumerable<Teacher> updatedTeachers);
        Task UpdateDisciplinesAsync(IEnumerable<Discipline> updatedDisciplines);
        Task UpdateGroupsAsync(IEnumerable<Group> updatedGroups);
        Task GenerateExamsAsync(int count);
        Task ClearGeneratedExamsAsync();
        Task<int> GetGeneratedExamsCountAsync();
        Task<ImportResult> ImportFromExcelAsync(DataType dataType, string filePath);
    }
}