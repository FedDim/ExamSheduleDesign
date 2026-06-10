using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services.Interfaces;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public class DisciplineRepository : IDisciplineRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAppLogger _logger;

        public DisciplineRepository(IDbConnectionFactory connectionFactory, IAppLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<List<Discipline>> GetAllAsync()
        {
            var list = new List<Discipline>();
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT Id, FullName, ShortName12, ShortName9, ShortName5 FROM Disciplines ORDER BY ShortName9";
                using var cmd = new SqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Discipline
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        ShortName12 = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        ShortName9 = reader.GetString(3),
                        ShortName5 = reader.IsDBNull(4) ? "" : reader.GetString(4)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DisciplineRepository.GetAllAsync", ex);
                throw;
            }
            return list;
        }

        public async Task<Discipline?> GetByIdAsync(int id)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT Id, FullName, ShortName12, ShortName9, ShortName5 FROM Disciplines WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Discipline
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        ShortName12 = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        ShortName9 = reader.GetString(3),
                        ShortName5 = reader.IsDBNull(4) ? "" : reader.GetString(4)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"DisciplineRepository.GetByIdAsync({id})", ex);
                throw;
            }
        }

        public async Task AddAsync(Discipline discipline)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = @"
            INSERT INTO Disciplines (FullName, ShortName12, ShortName9, ShortName5) 
            VALUES (@full, @s12, @s9, @s5);
            SELECT SCOPE_IDENTITY();";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@full", discipline.FullName ?? "");
                cmd.Parameters.AddWithValue("@s12", discipline.ShortName12 ?? "");
                cmd.Parameters.AddWithValue("@s9", discipline.ShortName9);
                cmd.Parameters.AddWithValue("@s5", discipline.ShortName5 ?? "");
                var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                discipline.Id = newId;
                _logger.Info($"Добавлена дисциплина: {discipline.ShortName9} с ID {newId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"DisciplineRepository.AddAsync ({discipline?.ShortName9})", ex);
                throw;
            }
        }

        public async Task UpdateAsync(Discipline discipline)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "UPDATE Disciplines SET FullName = @full, ShortName12 = @s12, ShortName9 = @s9, ShortName5 = @s5 WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@full", discipline.FullName ?? "");
                cmd.Parameters.AddWithValue("@s12", discipline.ShortName12 ?? "");
                cmd.Parameters.AddWithValue("@s9", discipline.ShortName9);
                cmd.Parameters.AddWithValue("@s5", discipline.ShortName5 ?? "");
                cmd.Parameters.AddWithValue("@id", discipline.Id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Info($"Обновлена дисциплина ID {discipline.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"DisciplineRepository.UpdateAsync ({discipline?.Id})", ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "DELETE FROM Disciplines WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Warning($"Удалена дисциплина ID {id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"DisciplineRepository.DeleteAsync ({id})", ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string shortName9)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT COUNT(*) FROM Disciplines WHERE ShortName9 = @s9";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@s9", shortName9);
                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"DisciplineRepository.ExistsAsync ({shortName9})", ex);
                return false;
            }
        }

        public async Task<bool> HasExamsAsync(int id)
        {
            using var conn = _connectionFactory.CreateLocalConnection();
            await conn.OpenAsync();
            const string sql = "SELECT COUNT(*) FROM Exams WHERE SubjectId = @id";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            long count = (long)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
}