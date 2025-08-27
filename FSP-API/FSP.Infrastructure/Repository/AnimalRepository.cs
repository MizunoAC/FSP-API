using FSP.Domain.Enums;
using FSP.Domain.Helpers;
using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using FSP.Domain.Models.Wrapper;
using FSP.Infrastructure.Repository.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RazorEngineCore;
using Sprache;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace FSP.Infrastructure.Repository
{
    public class AnimalsRepository : IAnimalRepository
    {
        private readonly string? _conn;
        private readonly IConfiguration _config;

        public AnimalsRepository(DbConnectionConfig con, IConfiguration config)
        {
            _conn = con.ConnectionString;
            _config = config;
        }

        #region UserAnimalRecord

        public async Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId)
        {
            var result = new MessageResponse();
            string base64String = model.img;
            byte[] imagenBytes = Convert.FromBase64String(base64String);

            using (SqlConnection conn = new SqlConnection(_conn))

            using (var cmd = new SqlCommand("[dbo].[InsertNewAnimalRecord]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CatalogId", model.CatalogId);
                cmd.Parameters.AddWithValue("@CommonNoun", model.CommonNoun);
                cmd.Parameters.AddWithValue("@AnimalState", model.AnimalState);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@img", imagenBytes);
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

        public async Task<AnimalRecordResponse> GetRecordsByUserId(string userId, string recordStatus, int pageNumber, int pageSize)
        {
            var result = new AnimalRecordResponse();
            var sql = ResourceHelper.GetResource("GetRecordsByUser");

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@RecordStatus", recordStatus);
                cmd.Parameters.AddWithValue("@PageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.AddWithValue("@PageSize", SqlDbType.Int).Value = pageSize;
                await conn.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var base64Image = "";
                    int.TryParse(reader["RecordId"].ToString(), out int recordId);
                    byte[]? binaryData = reader["Image"] != DBNull.Value ? (byte[])reader["Image"] : null;
                    if (binaryData != null)
                    {
                        var base64String = Convert.ToBase64String(binaryData);
                        base64Image = $"data:image/jpeg;base64,{base64String}";
                    }
                    result.Records.Add(new AnimalRecordDto
                    {
                        RecordId = recordId,
                        CommonNoun = reader["CommonNoun"].ToString(),
                        AnimalState = reader["AnimalState"].ToString(),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        img = base64Image
                    });
                }

                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        result.Pagination = new PaginationModel
                        {
                            Page = Convert.ToInt32(reader["page"]),
                            Size = Convert.ToInt32(reader["size"]),
                            Total = Convert.ToInt32(reader["total"]),
                            TotalPages = Convert.ToInt32(reader["totalPages"]),
                            HasNext = Convert.ToBoolean(reader["hasNext"]),
                            HasPrev = Convert.ToBoolean(reader["hasPrev"])
                        };
                    }
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
                
                return result;
            }
        }

        public async Task<AnimalRecordResponse> GetAllRecords(string recordStatus, int pageNumber, int pageSize)
        {
            var results = new AnimalRecordResponse();
            var sql = ResourceHelper.GetResource("GetAllRecords");

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@RecordStatus", recordStatus);
                cmd.Parameters.AddWithValue("@PageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.AddWithValue("@PageSize", SqlDbType.Int).Value = pageSize;
                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var base64Image = "";
                    int.TryParse(reader["RecordId"].ToString(), out int recordId);
                    byte[]? binaryData = reader["Image"] != DBNull.Value ? (byte[])reader["Image"] : null;
                    if (binaryData != null)
                    {
                        var base64String = Convert.ToBase64String(binaryData);
                        base64Image = $"data:image/jpeg;base64,{base64String}";
                    }

                    results.Records.Add(new AnimalRecordDto
                    {
                        RecordId = recordId,
                        CommonNoun = reader["CommonNoun"].ToString(),
                        AnimalState = reader["AnimalState"].ToString(),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        img = base64Image
                    });
                }

                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        results.Pagination = new PaginationModel
                        {
                            Page = Convert.ToInt32(reader["page"]),
                            Size = Convert.ToInt32(reader["size"]),
                            Total = Convert.ToInt32(reader["total"]),
                            TotalPages = Convert.ToInt32(reader["totalPages"]),
                            HasNext = Convert.ToBoolean(reader["hasNext"]),
                            HasPrev = Convert.ToBoolean(reader["hasPrev"])
                        };
                    }
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return results;
        }

        #endregion

        #region Catalog

        public async Task<CatalogResponse> GetCatalog(int pageNumber, int pageSize)
        {
            var results = new CatalogResponse();
            var sql = ResourceHelper.GetResource("GetCatalog");
            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                await conn.OpenAsync();
                cmd.Parameters.AddWithValue("@PageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.AddWithValue("@PageSize", SqlDbType.Int).Value = pageSize;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var base64Image = "";
                    byte[]? binaryData = reader["Image"] != DBNull.Value ? (byte[])reader["Image"] : null;
                    var base64String = Convert.ToBase64String(binaryData);
                    base64Image = $"data:image/jpeg;base64,{base64String}";
                    int.TryParse(reader["CatalogId"].ToString(), out int catalogId);
                    results.Catalog.Add(new CatalogDto
                    {
                        CatalogId = catalogId,
                        Specie = reader["Specie"].ToString(),
                        CommonNoun = reader["CommonNoun"].ToString(),
                        Description = reader["Description"].ToString(),
                        Habitat = reader["Habitat"].ToString(),
                        Habits = reader["Habits"].ToString(),
                        Reproduction = reader["Reproduction"].ToString(),
                        Distribution = reader["Distribution"].ToString(),
                        Feeding = reader["Feeding"].ToString(),
                        Category = reader["Category"].ToString(),
                        Image = base64Image
                    });
                }
                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        results.Pagination = new PaginationModel
                        {
                            Page = Convert.ToInt32(reader["page"]),
                            Size = Convert.ToInt32(reader["size"]),
                            Total = Convert.ToInt32(reader["total"]),
                            TotalPages = Convert.ToInt32(reader["totalPages"]),
                            HasNext = Convert.ToBoolean(reader["hasNext"]),
                            HasPrev = Convert.ToBoolean(reader["hasPrev"])
                        };
                    }
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
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
                    var base64Image = "";
                    byte[]? binaryData = reader["Image"] != DBNull.Value ? (byte[])reader["Image"] : null;

                    if (binaryData != null)
                    {
                        var base64String = Convert.ToBase64String(binaryData);
                        base64Image = $"data:image/jpeg;base64,{base64String}";
                    }
                    int.TryParse(reader["CatalogId"].ToString(), out int catalogId);

                    result.CatalogId = catalogId;
                    result.Specie = reader["Specie"].ToString();
                    result.CommonNoun = reader["CommonNoun"].ToString();
                    result.Description = reader["Description"].ToString();
                    result.Habitat = reader["Habitat"].ToString();
                    result.Habits = reader["Habits"].ToString();
                    result.Reproduction = reader["Reproduction"].ToString();
                    result.Distribution = reader["Distribution"].ToString();
                    result.Feeding = reader["Feeding"].ToString();
                    result.Category = reader["Category"].ToString();
                    result.Image = base64Image;
                }

                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public async Task<CatalogMapDto> GetCatalogMap(int catalogId)
        {
            var result = new CatalogMapDto();
            var sql = ResourceHelper.GetResource("GetCatalogMap");
            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CatalogId", catalogId);

                await conn.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var map = new CatalogMapCords
                    {
                        Location = reader["Location"].ToString()
                    };

                    result.Cords.Add(map);
                }

                result.CatalogId = catalogId;
                await reader.DisposeAsync();
                await conn.CloseAsync();
            }
            return result;
        }

        public async Task<IList<CatalogCommonNoun>> GetCommounName()
        {
            var result = new List<CatalogCommonNoun>();
            var sql = ResourceHelper.GetResource("GetCommonNoun");
            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();

                await conn.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int.TryParse(reader["CatalogId"].ToString(), out int catalogId);
                    var commounNoun = new CatalogCommonNoun
                    {
                        CommonNoun = reader["CommonNoun"].ToString(),
                        CatalogId = catalogId
                    };

                    result.Add(commounNoun);
                }

                await reader.DisposeAsync();
                await conn.CloseAsync();
            }
            return result;
        }
        #endregion
    }
}