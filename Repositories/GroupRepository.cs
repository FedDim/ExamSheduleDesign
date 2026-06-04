using ExamSheduleDesign.Models;
using ExamSheduleDesign.Services.Interfaces;
using ExamSheduleDesign.Services.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ExamSheduleDesign.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAppLogger _logger;

        public GroupRepository(IDbConnectionFactory connectionFactory, IAppLogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<List<Group>> GetAllAsync()
        {
            var groups = new List<Group>();
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT Id, Name, Department FROM Groups ORDER BY Name";
                using var cmd = new SqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    groups.Add(new Group
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Department = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GroupRepository.GetAllAsync", ex);
                throw;
            }
            return groups;
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT Id, Name, Department FROM Groups WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Group
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Department = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"GroupRepository.GetByIdAsync({id})", ex);
                throw;
            }
        }

        public async Task AddAsync(Group group)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = @"
            INSERT INTO Groups (Name, Department) 
            VALUES (@name, @dept);
            SELECT SCOPE_IDENTITY();";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", group.Name);
                cmd.Parameters.AddWithValue("@dept", group.Department ?? "");
                var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                group.Id = newId;
                _logger.Info($"Добавлена группа: {group.Name} с ID {newId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"GroupRepository.AddAsync ({group?.Name})", ex);
                throw;
            }
        }

        public async Task UpdateAsync(Group group)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "UPDATE Groups SET Name = @name, Department = @dept WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", group.Name);
                cmd.Parameters.AddWithValue("@dept", group.Department ?? "");
                cmd.Parameters.AddWithValue("@id", group.Id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Info($"Обновлена группа ID {group.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"GroupRepository.UpdateAsync ({group?.Id})", ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "DELETE FROM Groups WHERE Id = @id";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
                _logger.Warning($"Удалена группа ID {id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"GroupRepository.DeleteAsync ({id})", ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            try
            {
                using var conn = _connectionFactory.CreateServerConnection();
                await conn.OpenAsync();
                const string sql = "SELECT COUNT(*) FROM Groups WHERE Name = @name";
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"GroupRepository.ExistsAsync ({name})", ex);
                return false;
            }
        }
    }
}