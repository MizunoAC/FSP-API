using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Domain.Models.DTO;
using FSP.Domain.Helpers;
using System.Data;
using FSP.Domain.Models.Wrapper;

namespace FSP.Infrastructure.Repository
{
    public class AnimalsRepository : IAnimalRepository
    {
        private readonly string? _conn;
        public AnimalsRepository(DbConnectionConfig config)
        {
            _conn = config.ConnectionString;
        }

        #region UserAnimalRecord

        public async Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId)
        {
            var result = new MessageResponse();

            using (SqlConnection conn = new SqlConnection(_conn))

            using (var cmd = new SqlCommand("[dbo].[InsertNewAnimalRecord]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CommonNoun", model.CommonNoun);
                cmd.Parameters.AddWithValue("@AnimalState", model.AnimalState);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@img", model.img);
                cmd.Parameters.AddWithValue("@Location", model.Location);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Message = reader["Message"].ToString();
                    bool.TryParse(reader["IsError"].ToString(), out bool error);
                    result.Error = error;
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();

            }
            return result;
        }

        public async Task<List<AnimalRecordDto>> GetRecordsByUserId(string userId, string recordStatus)
        {
            var results = new List<AnimalRecordDto>();
            var sql = ResourceHelper.GetResource("GetRecordsByUser");

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@RecordStatus", recordStatus);
                await conn.OpenAsync();
                
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new AnimalRecordDto
                    {
                        CommonNoun = reader["CommonNoun"].ToString(),
                        AnimalState = reader["AnimalState"].ToString(),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString()
                    });
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return results;
        }

        #endregion

        #region Catalog

        public async Task<MessageResponse> InsertNewCatalog(CatalogRequest model)
        {
            var result = new MessageResponse();
            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("[dbo].[InsertNewAnimalCatalog]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Specie", model.Specie);
                cmd.Parameters.AddWithValue("@CommonNoun", model.CommonNoun);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Habits", model.Habits);
                cmd.Parameters.AddWithValue("@Habitat", model.Habitat);
                cmd.Parameters.AddWithValue("@Reproduction", model.Reproduction);
                cmd.Parameters.AddWithValue("@Distribution", model.Distribution);
                cmd.Parameters.AddWithValue("@Feeding", model.Feeding);
                cmd.Parameters.AddWithValue("@Category", model.Category);
                cmd.Parameters.AddWithValue("@Map", model.Map);
                cmd.Parameters.AddWithValue("@Image", model.Image);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Message = reader["Message"].ToString();
                    bool.TryParse(reader["IsError"].ToString(), out bool error);
                    result.Error = error;
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public async Task<List<CatalogDto>> GetCatalog()
        {
            var results = new List<CatalogDto>();
            var sql = ResourceHelper.GetResource("GetCatalog");
            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new CatalogDto
                    {
                        Specie = reader["Specie"].ToString(),
                        CommonNoun = reader["CommonNoun"].ToString(),
                        Description = reader["Description"].ToString(),
                        Habitat = reader["Habitat"].ToString(),
                        Habits = reader["Habits"].ToString(),
                        Reproduction = reader["Reproduction"].ToString(),
                        Distribution = reader["Distribution"].ToString(),
                        Feeding = reader["Feeding"].ToString(),
                        Category = reader["Category"].ToString(),
                        Image = reader["Image"].ToString(),
                        Map = reader["Map"].ToString()
                    });
                }
            }
            return results;
        }

        public async Task<CatalogDto> GetCatalogByCommonNoun(string commonNoun)
        {
            var result = new CatalogDto();
            var sql = ResourceHelper.GetResource("GetCatalogByCommonNoun");
            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CommonNoun", commonNoun);
                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Specie = reader["Specie"].ToString();
                    result.CommonNoun = reader["CommonNoun"].ToString();
                    result.Description = reader["Description"].ToString();
                    result.Habitat = reader["Habitat"].ToString();
                    result.Habits = reader["Habits"].ToString();
                    result.Reproduction = reader["Reproduction"].ToString();
                    result.Distribution = reader["Distribution"].ToString();
                    result.Feeding = reader["Feeding"].ToString();
                    result.Category = reader["Category"].ToString();
                    result.Image = reader["Image"].ToString();
                    result.Map = reader["Map"].ToString();
                }
            }
            return result;
        }
        #endregion
    }
}