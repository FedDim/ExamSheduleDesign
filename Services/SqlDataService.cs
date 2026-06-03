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

        public SqlDataService(ITeacherRepository teacherRepo, IDisciplineRepository disciplineRepo,
                              IGroupRepository groupRepo, IExamRepository examRepo, IAppLogger logger,
                              INotificationService notificationService)
        {
            _teacherRepo = teacherRepo;
            _disciplineRepo = disciplineRepo;
            _groupRepo = groupRepo;
            _examRepo = examRepo;
            _logger = logger;
            _notificationService = notificationService;
        }

        // ========== Асинхронные методы для вызова из ViewModel ==========
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

        // ========== Синхронные методы для IDataService (обёртки через Task.Run) ==========
        public List<Teacher> GetTeachers() => Task.Run(GetTeachersAsync).Result;
        public List<Discipline> GetDisciplines() => Task.Run(GetDisciplinesAsync).Result;
        public List<Group> GetGroups() => Task.Run(GetGroupsAsync).Result;
        public List<Exam> GetExams() => Task.Run(GetExamsAsync).Result;

        public void AddExam(Exam exam)
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
                _examRepo.AddAsync(dto).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error($"AddExam failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при сохранении экзамена.");
            }
        }

        public void RemoveExams(IEnumerable<Exam> exams)
        {
            foreach (var exam in exams)
            {
                try
                {
                    if (exam.Id > 0)
                        _examRepo.DeleteAsync(exam.Id).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error($"RemoveExam {exam.Id} failed", ex);
                    _notificationService.Show("Ошибка при удалении экзамена.");
                }
            }
        }

        public void AddTeacher(Teacher teacher)
        {
            try
            {
                _teacherRepo.AddAsync(teacher).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error($"AddTeacher failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при добавлении преподавателя.");
            }
        }

        public void AddDiscipline(Discipline discipline)
        {
            try
            {
                _disciplineRepo.AddAsync(discipline).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error($"AddDiscipline failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при добавлении дисциплины.");
            }
        }

        public void AddGroup(Group group)
        {
            try
            {
                _groupRepo.AddAsync(group).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error($"AddGroup failed: {ex.Message}", ex);
                _notificationService.Show("Ошибка при добавлении группы.");
            }
        }

        public void UpdateTeachers(IEnumerable<Teacher> updatedTeachers)
        {
            foreach (var teacher in updatedTeachers)
            {
                try
                {
                    _teacherRepo.UpdateAsync(teacher).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateTeacher {teacher.Id} failed", ex);
                    _notificationService.Show("Ошибка при обновлении преподавателя.");
                }
            }
        }

        public void UpdateDisciplines(IEnumerable<Discipline> updatedDisciplines)
        {
            foreach (var discipline in updatedDisciplines)
            {
                try
                {
                    _disciplineRepo.UpdateAsync(discipline).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateDiscipline {discipline.Id} failed", ex);
                    _notificationService.Show("Ошибка при обновлении дисциплины.");
                }
            }
        }

        public void UpdateGroups(IEnumerable<Group> updatedGroups)
        {
            foreach (var group in updatedGroups)
            {
                try
                {
                    _groupRepo.UpdateAsync(group).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error($"UpdateGroup {group.Id} failed", ex);
                    _notificationService.Show("Ошибка при обновлении группы.");
                }
            }
        }

        public void GenerateExams(int count)
        {
            try
            {
                var teachers = GetTeachers();
                var disciplines = GetDisciplines();
                var groups = GetGroups();
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
                    AddExam(exam);
                }
                _notificationService.Show($"Сгенерировано {count} экзаменов.");
            }
            catch (Exception ex)
            {
                _logger.Error("GenerateExams failed", ex);
                _notificationService.Show("Ошибка при генерации экзаменов.");
            }
        }

        public void ClearGeneratedExams()
        {
            try
            {
                _examRepo.DeleteAllAsync().Wait();
                _notificationService.Show("Все экзамены удалены.");
            }
            catch (Exception ex)
            {
                _logger.Error("ClearGeneratedExams failed", ex);
                _notificationService.Show("Ошибка при очистке экзаменов.");
            }
        }

        public int GetGeneratedExamsCount()
        {
            try
            {
                return _examRepo.GetAllRawAsync().Result.Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}