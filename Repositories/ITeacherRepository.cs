using ExamSheduleDesign.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public interface ITeacherRepository
    {
        Task<List<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int id);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string name);
    }
}
