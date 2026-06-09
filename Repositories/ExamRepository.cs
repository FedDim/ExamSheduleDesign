using ExamSheduleDesign.Services.DTO;
using ExamSheduleDesign.Services.Interfaces;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAppLogger _logger;
        private bool _tableChecked = false;
        private readonly object _tableLock = new object();

        public ExamRepository(IDbConnectionFactory connectionFactory, IAppLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        private async Task EnsureTableExistsAsync()
        {
            if (_tableChecked) return;
            lock (_tableLock)
            {
                if (_tableChecked) return;
                try
                {
                    using var conn = _connectionFactory.CreateLocalConnection();
                    conn.Open();
                    const string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS Exams (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Teacher1Id INTEGER,
                            Teacher2Id INTEGER,
                            SubjectId INTEGER,
                            GroupId INTEGER,
                            Classroom TEXT,
                            Department TEXT,
                            ExamDate TEXT,
                            ExamTime TEXT,
                            ExamType TEXT
                        )";
                    using var cmd = new SQLiteCommand(createTableSql, conn);
                    cmd.ExecuteNonQuery();
                    _tableChecked = true;
                    _logger.Info("Таблица Exams проверена/создана в SQLite");
                }
                catch (Exception ex)
                {
                    _logger.Error("Ошибка при создании таблицы Exams", ex);
                    throw;
                }
            }
        }

        public async Task<List<ExamScheduleDto>> GetAllRawAsync()
        {
            await EnsureTableExistsAsync();
            var list = new List<ExamScheduleDto>();
            try
            {
                using var conn = _connectionFactory.CreateLocalConnection();
                await conn.OpenAsync();
                const string sql = @"
                    SELECT Id, Teacher1Id, Teacher2Id, SubjectId, GroupId, 
                           Classroom, Department, ExamDate, ExamTime, ExamType 
                    FROM Exams";
                using var cmd = new SQLiteCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new ExamScheduleDto
                    {
                        Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        Teacher1Id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                        Teacher2Id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                        SubjectId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                        GroupId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                        Classroom = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        DepartmentName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        ExamDate = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        ExamTime = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        ExamType = reader.IsDBNull(9) ? "" : reader.GetString(9)
                    });
                }
                _logger.Info($"Загружено {list.Count} экзаменов из SQLite");
                return list;
            }
            catch (Exception ex)
            {
                _logger.Error("ExamRepository.GetAllRawAsync", ex);
                throw;
            }
        }

        public async Task AddAsync(ExamScheduleDto exam)
        {
            await EnsureTableExistsAsync();
            try
            {
                using var conn = _connectionFactory.CreateLocalConnection();
                await conn.OpenAsync();
                const string sql = @"
                    INSERT INTO Exams (Teacher1Id, Teacher2Id, SubjectId, GroupId, 
                                       Classroom, Department, ExamDate, ExamTime, ExamType) 
                    VALUES (@Teacher1Id, @Teacher2Id, @SubjectId, @GroupId, 
                            @Classroom, @Department, @ExamDate, @ExamTime, @ExamType)";
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Teacher1Id", exam.Teacher1Id);
                cmd.Parameters.AddWithValue("@Teacher2Id", exam.Teacher2Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectId", exam.SubjectId);
                cmd.Parameters.AddWithValue("@GroupId", exam.GroupId);
                cmd.Parameters.AddWithValue("@Classroom", exam.Classroom ?? "");
                cmd.Parameters.AddWithValue("@Department", exam.DepartmentName ?? "");
                cmd.Parameters.AddWithValue("@ExamDate", exam.ExamDate ?? "");
                cmd.Parameters.AddWithValue("@ExamTime", exam.ExamTime ?? "");
                cmd.Parameters.AddWithValue("@ExamType", exam.ExamType ?? "");
                await cmd.ExecuteNonQueryAsync();
                _logger.Info($"Экзамен сохранён: {exam.ExamDate} {exam.ExamTime}");
            }
            catch (Exception ex)
            {
                _logger.Error("ExamRepository.AddAsync", ex);
                throw;
            }
        }

        public async Task UpdateAsync(ExamScheduleDto exam)
        {
            await EnsureTableExistsAsync();
            try
            {
                using var conn = _connectionFactory.CreateLocalConnection();
                await conn.OpenAsync();
                const string sql = @"
                    UPDATE Exams SET
                        Teacher1Id = @Teacher1Id,
                        Teacher2Id = @Teacher2Id,
                        SubjectId = @SubjectId,
                        GroupId = @GroupId,
                        Classroom = @Classroom,
                        Department = @Department,
                        ExamDate = @ExamDate,
                        ExamTime = @ExamTime,
                        ExamType = @ExamType
                    WHERE Id = @Id";
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Teacher1Id", exam.Teacher1Id);
                cmd.Parameters.AddWithValue("@Teacher2Id", exam.Teacher2Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectId", exam.SubjectId);
                cmd.Parameters.AddWithValue("@GroupId", exam.GroupId);
                cmd.Parameters.AddWithValue("@Classroom", exam.Classroom ?? "");
                cmd.Parameters.AddWithValue("@Department", exam.DepartmentName ?? "");
                cmd.Parameters.AddWithValue("@ExamDate", exam.ExamDate ?? "");
                cmd.Parameters.AddWithValue("@ExamTime", exam.ExamTime ?? "");
                cmd.Parameters.AddWithValue("@ExamType", exam.ExamType ?? "");
                cmd.Parameters.AddWithValue("@Id", exam.Id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Info($"Обновлён экзамен ID {exam.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"ExamRepository.UpdateAsync ({exam.Id})", ex);
                throw;
            }
        }

        public async Task DeleteAsync(int examId)
        {
            await EnsureTableExistsAsync();
            try
            {
                using var conn = _connectionFactory.CreateLocalConnection();
                await conn.OpenAsync();
                const string sql = "DELETE FROM Exams WHERE Id = @id";
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", examId);
                await cmd.ExecuteNonQueryAsync();
                _logger.Info($"Удалён экзамен ID {examId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"ExamRepository.DeleteAsync({examId})", ex);
                throw;
            }
        }

        public async Task DeleteAllAsync()
        {
            await EnsureTableExistsAsync();
            try
            {
                using var conn = _connectionFactory.CreateLocalConnection();
                await conn.OpenAsync();
                const string sql = "DELETE FROM Exams; DELETE FROM sqlite_sequence WHERE name='Exams';";
                using var cmd = new SQLiteCommand(sql, conn);
                await cmd.ExecuteNonQueryAsync();
                _logger.Warning("Все экзамены удалены из SQLite, счётчик сброшен");
            }
            catch (Exception ex)
            {
                _logger.Error("ExamRepository.DeleteAllAsync", ex);
                throw;
            }
        }

        public async Task<int> GetCountAsync()
        {
            await EnsureTableExistsAsync();
            using var conn = _connectionFactory.CreateLocalConnection();
            await conn.OpenAsync();
            const string sql = "SELECT COUNT(*) FROM Exams";
            using var cmd = new SQLiteCommand(sql, conn);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
    }
}