using ExamSheduleDesign.Models;
using ExamSheduleDesign.Repositories;
using ExamSheduleDesign.Services.DTO;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class SqlDataService : IDataService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IDisciplineRepository _disciplineRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IExamRepository _examRepo;
        private readonly IAppLogger _logger;
        private readonly INotificationService _notificationService;
        private static bool? _serverAvailable = null;
        private readonly DataImporter _dataImporter;

        public SqlDataService(ITeacherRepository teacherRepo, IDisciplineRepository disciplineRepo,
                              IGroupRepository groupRepo, IExamRepository examRepo, IAppLogger logger,
                              INotificationService notificationService, DataImporter dataImporter)
        {
            _teacherRepo = teacherRepo;
            _disciplineRepo = disciplineRepo;
            _groupRepo = groupRepo;
            _examRepo = examRepo;
            _logger = logger;
            _notificationService = notificationService;
            _dataImporter = dataImporter;
        }

        // Асинхронные методы
        public async Task<List<Teacher>> GetTeachersAsync()
        {
            if (_serverAvailable == false) return new List<Teacher>();
            try
            {
                var result = await _teacherRepo.GetAllAsync();
                _serverAvailable = true;
                return result;
            }
            catch (Exception ex)
            {
                _serverAvailable = false;
                _logger.Error("GetTeachersAsync - сервер недоступен", ex);
                return new List<Teacher>();
            }
        }

        public async Task<List<Discipline>> GetDisciplinesAsync()
        {
            if (_serverAvailable == false) return new List<Discipline>();
            try
            {
                var result = await _disciplineRepo.GetAllAsync();
                _serverAvailable = true;
                return result;
            }
            catch (Exception ex)
            {
                _serverAvailable = false;
                _logger.Error("GetDisciplinesAsync - сервер недоступен", ex);
                return new List<Discipline>();
            }
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            if (_serverAvailable == false) return new List<Group>();
            try
            {
                var result = await _groupRepo.GetAllAsync();
                _serverAvailable = true;
                return result;
            }
            catch (Exception ex)
            {
                _serverAvailable = false;
                _logger.Error("GetGroupsAsync - сервер недоступен", ex);
                return new List<Group>();
            }
        }

        public async Task<List<Exam>> GetExamsAsync()
        {
            return await GetAllExamsWithDetailsAsync();
        }

        private async Task<List<Exam>> GetAllExamsWithDetailsAsync()
        {
            try
            {
                var dtos = await _examRepo.GetAllRawAsync();
                if (dtos.Count == 0) return new List<Exam>();

                var teachers = await GetTeachersAsync();
                var disciplines = await GetDisciplinesAsync();
                var groups = await GetGroupsAsync();

                var teacherDict = teachers.ToDictionary(t => t.Id);
                var disciplineDict = disciplines.ToDictionary(d => d.Id);
                var groupDict = groups.ToDictionary(g => g.Id);

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
                return exams;
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllExamsWithDetailsAsync", ex);
                return new List<Exam>();
            }
        }

        public async Task AddExamAsync(Exam exam)
        {
            try
            {
                var dto = new ExamScheduleDto
                {
                    Teacher1Id = exam.Teacher1?.Id ?? 0,
                    Teacher2Id = exam.Teacher2?.Id,
                    SubjectId = exam.Discipline?.Id ?? 0,
                    GroupId = exam.Group?.Id ?? 0,
                    Classroom = exam.Classroom ?? "",
                    DepartmentName = exam.Group?.Department ?? "",
                    ExamDate = exam.Date.ToString("yyyy-MM-dd"),
                    ExamTime = exam.Time,
                    ExamType = exam.Type
                };
                await _examRepo.AddAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.Error($"AddExamAsync failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при сохранении экзамена.");
            }
        }

        public async Task RemoveExamsAsync(IEnumerable<Exam> exams)
        {
            foreach (var exam in exams)
            {
                try
                {
                    if (exam.Id > 0)
                        await _examRepo.DeleteAsync(exam.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"RemoveExamAsync {exam.Id} failed", ex);
                    _notificationService.Show("Ошибка при удалении экзамена.");
                }
            }
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            try
            {
                await _teacherRepo.AddAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.Error($"AddTeacherAsync failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при добавлении преподавателя.");
            }
        }

        public async Task AddDisciplineAsync(Discipline discipline)
        {
            try
            {
                await _disciplineRepo.AddAsync(discipline);
            }
            catch (Exception ex)
            {
                _logger.Error($"AddDisciplineAsync failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при добавлении дисциплины.");
            }
        }

        public async Task AddGroupAsync(Group group)
        {
            try
            {
                await _groupRepo.AddAsync(group);
            }
            catch (Exception ex)
            {
                _logger.Error($"AddGroupAsync failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при добавлении группы.");
            }
        }

        public async Task UpdateTeachersAsync(IEnumerable<Teacher> updatedTeachers)
        {
            foreach (var teacher in updatedTeachers)
            {
                try
                {
                    await _teacherRepo.UpdateAsync(teacher);
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateTeacherAsync {teacher.Id} failed", ex);
                    _notificationService.Show("Ошибка при обновлении преподавателя.");
                }
            }
        }

        public async Task UpdateDisciplinesAsync(IEnumerable<Discipline> updatedDisciplines)
        {
            foreach (var discipline in updatedDisciplines)
            {
                try
                {
                    await _disciplineRepo.UpdateAsync(discipline);
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateDisciplineAsync {discipline.Id} failed", ex);
                    _notificationService.Show("Ошибка при обновлении дисциплины.");
                }
            }
        }

        public async Task UpdateGroupsAsync(IEnumerable<Group> updatedGroups)
        {
            foreach (var group in updatedGroups)
            {
                try
                {
                    await _groupRepo.UpdateAsync(group);
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateGroupAsync {group.Id} failed", ex);
                    _notificationService.Show("Ошибка при обновлении группы.");
                }
            }
        }

        public async Task GenerateExamsAsync(int count)
        {
            try
            {
                var teachers = await GetTeachersAsync();
                var disciplines = await GetDisciplinesAsync();
                var groups = await GetGroupsAsync();
                if (teachers.Count == 0 || disciplines.Count == 0 || groups.Count == 0)
                {
                    _notificationService.Show("Невозможно сгенерировать экзамены: отсутствуют справочные данные (проверьте подключение к серверу).");
                    return;
                }

                var random = new Random();
                var times = new[] { "9:00", "10:40", "11:00", "13:00", "13:30", "14:35", "15:00", "16:20" };
                var rooms = new[] { "301", "205", "402", "110", "215", "101", "305" };
                var types = new[] { "Экзамен", "Консультация" };
                var startDate = DateTime.Today.AddDays(7);

                for (int i = 0; i < count; i++)
                {
                    var exam = new Exam
                    {
                        Date = startDate.AddDays(random.Next(0, 14)),
                        Time = times[random.Next(times.Length)],
                        Type = types[random.Next(types.Length)],
                        Teacher1 = teachers[random.Next(teachers.Count)],
                        Teacher2 = random.Next(2) == 0 ? null : teachers[random.Next(teachers.Count)],
                        Discipline = disciplines[random.Next(disciplines.Count)],
                        Group = groups[random.Next(groups.Count)],
                        Classroom = rooms[random.Next(rooms.Length)],
                        IsSelected = false
                    };
                    await AddExamAsync(exam);
                }
                _notificationService.Show($"Сгенерировано {count} экзаменов.");
            }
            catch (Exception ex)
            {
                _logger.Error("GenerateExamsAsync failed", ex);
                _notificationService.Show("Ошибка при генерации экзаменов.");
            }
        }

        public async Task ClearGeneratedExamsAsync()
        {
            try
            {
                await _examRepo.DeleteAllAsync();
                _notificationService.Show("Все экзамены удалены.");
            }
            catch (Exception ex)
            {
                _logger.Error("ClearGeneratedExamsAsync failed", ex);
                _notificationService.Show("Ошибка при очистке экзаменов.");
            }
        }

        public async Task<int> GetGeneratedExamsCountAsync()
        {
            try
            {
                var list = await _examRepo.GetAllRawAsync();
                return list.Count;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<ImportResult> ImportFromExcelAsync(DataType dataType, string filePath)
        {
            return await _dataImporter.ImportAsync(dataType, filePath);
        }
    }
}