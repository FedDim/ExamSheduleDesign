using ClosedXML.Excel;
using ExamSheduleDesign.Models;
using ExamSheduleDesign.Repositories;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Services
{
    public class ImportResult
    {
        public int Added { get; set; }
        public int Skipped { get; set; }
        public int ErrorsCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public string GetSummary()
        {
            return $"Импорт завершён:\n\n" +
                   $"Добавлено новых записей: {Added}\n" +
                   $"Пропущено (уже существуют): {Skipped}\n" +
                   $"Ошибок: {ErrorsCount}";
        }
    }

    public class DataImporter
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IDisciplineRepository _disciplineRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IAppLogger _logger;

        public DataImporter(ITeacherRepository teacherRepo, IDisciplineRepository disciplineRepo,
                            IGroupRepository groupRepo, IAppLogger logger)
        {
            _teacherRepo = teacherRepo;
            _disciplineRepo = disciplineRepo;
            _groupRepo = groupRepo;
            _logger = logger;
        }

        public async Task<ImportResult> ImportAsync(DataType dataType, string excelFilePath)
        {
            var result = new ImportResult();

            if (!File.Exists(excelFilePath))
            {
                result.ErrorMessages.Add("Файл не найден.");
                result.ErrorsCount = 1;
                return result;
            }

            try
            {
                using var workbook = new XLWorkbook(excelFilePath);
                var worksheet = workbook.Worksheet(1);
                int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                for (int rowNum = 2; rowNum <= rowCount; rowNum++)
                {
                    var row = worksheet.Row(rowNum);
                    try
                    {
                        switch (dataType)
                        {
                            case DataType.TEACHER:
                                await ImportTeacherAsync(row, result);
                                break;
                            case DataType.GROUP:
                                await ImportGroupAsync(row, result);
                                break;
                            case DataType.DISCIPLINE:
                                await ImportDisciplineAsync(row, result);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.ErrorsCount++;
                        result.ErrorMessages.Add($"Строка {rowNum}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка импорта {dataType}", ex);
                result.ErrorMessages.Add($"Ошибка чтения файла: {ex.Message}");
                result.ErrorsCount++;
            }

            return result;
        }

        private async Task ImportTeacherAsync(IXLRow row, ImportResult result)
        {
            string name = row.Cell(1).GetString().Trim();
            if (string.IsNullOrEmpty(name)) return;

            if (await _teacherRepo.ExistsAsync(name))
            {
                result.Skipped++;
                return;
            }

            var teacher = new Teacher
            {
                Name = name,
                Classroom = row.Cell(2).GetString().Trim(),
                AcademicBuilding = int.TryParse(row.Cell(3).GetString(), out int b) ? b : 0
            };
            await _teacherRepo.AddAsync(teacher);
            result.Added++;
        }

        private async Task ImportGroupAsync(IXLRow row, ImportResult result)
        {
            string name = row.Cell(1).GetString().Trim();
            if (string.IsNullOrEmpty(name)) return;

            if (await _groupRepo.ExistsAsync(name))
            {
                result.Skipped++;
                return;
            }

            var group = new Group
            {
                Name = name,
                Department = row.Cell(2).GetString().Trim()
            };
            await _groupRepo.AddAsync(group);
            result.Added++;
        }

        private async Task ImportDisciplineAsync(IXLRow row, ImportResult result)
        {
            string shortName9 = row.Cell(1).GetString().Trim();
            if (string.IsNullOrEmpty(shortName9)) return;

            if (await _disciplineRepo.ExistsAsync(shortName9))
            {
                result.Skipped++;
                return;
            }

            var discipline = new Discipline
            {
                ShortName9 = shortName9,
                FullName = row.Cell(2).GetString().Trim(),
                ShortName12 = row.Cell(3).GetString().Trim(),
                ShortName5 = row.Cell(4).GetString().Trim()
            };
            await _disciplineRepo.AddAsync(discipline);
            result.Added++;
        }
    }
}