using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services.Interfaces;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAppLogger _logger;

        public TeacherRepository(IDbConnectionFactory connectionFactory, IAppLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<List<Teacher>> GetAllAsync()
        {
            var teachers = new List<Teacher>();
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT Id, Name, Classroom, AcademicBuilding FROM Teachers ORDER BY Name";
                using var cmd = new SqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    teachers.Add(new Teacher
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Classroom = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        AcademicBuilding = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error("TeacherRepository.GetAllAsync", ex);
                throw;
            }
            return teachers;
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT Id, Name, Classroom, AcademicBuilding FROM Teachers WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Teacher
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Classroom = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        AcademicBuilding = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"TeacherRepository.GetByIdAsync({id})", ex);
                throw;
            }
        }

        public async Task AddAsync(Teacher teacher)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = @"
            INSERT INTO Teachers (Name, Classroom, AcademicBuilding) 
            VALUES (@name, @classroom, @building);
            SELECT SCOPE_IDENTITY();";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", teacher.Name);
                cmd.Parameters.AddWithValue("@classroom", teacher.Classroom ?? "");
                cmd.Parameters.AddWithValue("@building", teacher.AcademicBuilding);
                var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                teacher.Id = newId;
                _logger.Info($"Добавлен преподаватель: {teacher.Name} с ID {newId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"TeacherRepository.AddAsync ({teacher?.Name})", ex);
                throw;
            }
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "UPDATE Teachers SET Name = @name, Classroom = @classroom, AcademicBuilding = @building WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", teacher.Name);
                cmd.Parameters.AddWithValue("@classroom", teacher.Classroom ?? "");
                cmd.Parameters.AddWithValue("@building", teacher.AcademicBuilding);
                cmd.Parameters.AddWithValue("@id", teacher.Id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Info($"Обновлён преподаватель ID {teacher.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"TeacherRepository.UpdateAsync ({teacher?.Id})", ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "DELETE FROM Teachers WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Warning($"Удалён преподаватель ID {id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"TeacherRepository.DeleteAsync ({id})", ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT COUNT(*) FROM Teachers WHERE Name = @name";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"TeacherRepository.ExistsAsync ({name})", ex);
                return false;
            }
        }
    }
}