using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services.DTO;
using ExamSheduleDesign.Services.Interfaces;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class BufferService : IBufferService
    {
        private readonly IDataService _dataService;
        private readonly IAppLogger _logger;
        private readonly string _bufferFilePath;

        public BufferService(IDataService dataService, IAppLogger logger)
        {
            _dataService = dataService;
            _logger = logger;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(baseDir, "Data");
            Directory.CreateDirectory(dataFolder);
            _bufferFilePath = Path.Combine(dataFolder, "buffer.json");
        }

        public async Task SaveBufferAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var exams = await _dataService.GetExamsAsync();
                    var bufferDtos = exams.Select(exam => new BufferExamDto
                    {
                        Id = exam.Id,
                        Date = exam.Date,
                        Time = exam.Time,
                        Type = exam.Type,
                        Teacher1Id = exam.Teacher1?.Id ?? 0,
                        Teacher2Id = exam.Teacher2?.Id,
                        DisciplineId = exam.Discipline?.Id ?? 0,
                        GroupId = exam.Group?.Id ?? 0,
                        Classroom = exam.Classroom,
                        IsSelected = exam.IsSelected
                    }).ToList();

                    var json = JsonSerializer.Serialize(bufferDtos, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_bufferFilePath, json);
                    _logger.Info($"Буфер сохранён: {bufferDtos.Count} экзаменов");
                }
                catch (Exception ex)
                {
                    _logger.Error("Ошибка при сохранении буфера", ex);
                    throw;
                }
            });
        }

        public async Task LoadBufferAsync()
        {
            if (!await HasBufferAsync())
                throw new InvalidOperationException("Буфер пуст или не существует.");

            await Task.Run(async () =>
            {
                try
                {
                    string json = File.ReadAllText(_bufferFilePath);
                    var bufferDtos = JsonSerializer.Deserialize<List<BufferExamDto>>(json);
                    if (bufferDtos == null || bufferDtos.Count == 0) return;

                    await _dataService.ClearGeneratedExamsAsync();

                    var teachers = await _dataService.GetTeachersAsync();
                    var disciplines = await _dataService.GetDisciplinesAsync();
                    var groups = await _dataService.GetGroupsAsync();

                    var teacherDict = teachers.ToDictionary(t => t.Id);
                    var disciplineDict = disciplines.ToDictionary(d => d.Id);
                    var groupDict = groups.ToDictionary(g => g.Id);

                    var examsToAdd = new List<Exam>();
                    foreach (var dto in bufferDtos)
                    {
                        var exam = new Exam
                        {
                            Date = dto.Date,
                            Time = dto.Time,
                            Type = dto.Type,
                            Teacher1 = teacherDict.TryGetValue(dto.Teacher1Id, out var t1) ? t1 : null,
                            Teacher2 = dto.Teacher2Id.HasValue && teacherDict.TryGetValue(dto.Teacher2Id.Value, out var t2) ? t2 : null,
                            Discipline = disciplineDict.TryGetValue(dto.DisciplineId, out var disc) ? disc : null,
                            Group = groupDict.TryGetValue(dto.GroupId, out var grp) ? grp : null,
                            Classroom = dto.Classroom,
                            IsSelected = false
                        };
                        examsToAdd.Add(exam);
                    }

                    foreach (var exam in examsToAdd)
                        await _dataService.AddExamAsync(exam);

                    _logger.Info($"Буфер загружен: {examsToAdd.Count} экзаменов восстановлено");
                }
                catch (Exception ex)
                {
                    _logger.Error("Ошибка при загрузке буфера", ex);
                    throw;
                }
            });
        }

        public async Task ClearBufferAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(_bufferFilePath))
                        File.Delete(_bufferFilePath);
                    _logger.Info("Буфер очищен (файл удалён)");
                }
                catch (Exception ex)
                {
                    _logger.Error("Ошибка при очистке буфера", ex);
                    throw;
                }
            });
        }

        public async Task<bool> HasBufferAsync()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(_bufferFilePath)) return false;
                try
                {
                    string json = File.ReadAllText(_bufferFilePath);
                    var bufferDtos = JsonSerializer.Deserialize<List<BufferExamDto>>(json);
                    return bufferDtos != null && bufferDtos.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<int> GetBufferCountAsync()
        {
            if (!await HasBufferAsync()) return 0;
            return await Task.Run(() =>
            {
                try
                {
                    string json = File.ReadAllText(_bufferFilePath);
                    var bufferDtos = JsonSerializer.Deserialize<List<BufferExamDto>>(json);
                    return bufferDtos?.Count ?? 0;
                }
                catch
                {
                    return 0;
                }
            });
        }
    }
}