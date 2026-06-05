using ExamSheduleDesign.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public interface IExamRepository
    {
        Task<List<ExamScheduleDto>> GetAllRawAsync();
        Task AddAsync(ExamScheduleDto exam);
        Task UpdateAsync(ExamScheduleDto exam);
        Task DeleteAsync(int examId);
        Task DeleteAllAsync();
    }
}