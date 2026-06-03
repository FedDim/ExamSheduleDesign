using ExamSheduleDesign.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllAsync();
        Task<Group?> GetByIdAsync(int id);
        Task AddAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string name);
    }
}
