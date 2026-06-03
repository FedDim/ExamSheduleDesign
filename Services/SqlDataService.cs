using ExamSheduleDesign.Models;
using ExamSheduleDesign.Repositories;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class SqlDataService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IDisciplineRepository _disciplineRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IExamRepository _examRepo;
        private readonly IAppLogger _logger;

        public SqlDataService(ITeacherRepository teacherRepo, IDisciplineRepository disciplineRepo,
                              IGroupRepository groupRepo, IExamRepository examRepo, IAppLogger logger)
        {
            _teacherRepo = teacherRepo;
            _disciplineRepo = disciplineRepo;
            _groupRepo = groupRepo;
            _examRepo = examRepo;
            _logger = logger;
        }

        public async Task<List<Exam>> GetAllExamsWithDetailsAsync()
        {
            try
            {
                // 1. Загружаем все DTO из SQLite
                var dtos = await _examRepo.GetAllRawAsync();
                if (dtos == null || dtos.Count == 0)
                    return new List<Exam>();

                // 2. Загружаем справочники из SQL Server
                var teachers = await _teacherRepo.GetAllAsync();
                var disciplines = await _disciplineRepo.GetAllAsync();
                var groups = await _groupRepo.GetAllAsync();

                var teacherDict = teachers.ToDictionary(t => t.Id);
                var disciplineDict = disciplines.ToDictionary(d => d.Id);
                var groupDict = groups.ToDictionary(g => g.Id);

                // 3. Собираем экзамены
                var exams = new List<Exam>();
                foreach (var dto in dtos)
                {
                    var exam = new Exam
                    {
                        Id = dto.Id,
                        Date = DateTime.TryParse(dto.ExamDate, out var date) ? date : DateTime.MinValue,
                        Time = dto.ExamTime,
                        Type = dto.ExamType,
                        Teacher1 = teacherDict.TryGetValue(dto.Teacher1Id, out var t1) ? t1 : null,
                        Teacher2 = dto.Teacher2Id.HasValue && teacherDict.TryGetValue(dto.Teacher2Id.Value, out var t2) ? t2 : null,
                        Discipline = disciplineDict.TryGetValue(dto.SubjectId, out var disc) ? disc : null,
                        Group = groupDict.TryGetValue(dto.GroupId, out var grp) ? grp : null,
                        Classroom = dto.Classroom
                    };
                    exams.Add(exam);
                }
                _logger.Info($"Собрано {exams.Count} экзаменов с подгрузкой справочников");
                return exams;
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllExamsWithDetailsAsync", ex);
                throw;
            }
        }
    }
}