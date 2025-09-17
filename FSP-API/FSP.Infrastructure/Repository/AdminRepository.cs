using FSP.Domain.Enums;
using FSP.Domain.Helpers;
using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using FSP.Domain.Models.Wrapper;
using FSP.Infrastructure.Repository.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RazorEngineCore;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace FSP.Infrastructure.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string? _conn;
        private readonly IConfiguration _config;

        public AdminRepository(DbConnectionConfig con, IConfiguration config)
        {
            _conn = con.ConnectionString;
            _config = config;
        }
        #region admin
        public async Task<MessageResponse> UpdateCatalog(CatalogRequestDto catalog, string userId)
        {
            var result = new MessageResponse();
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("[dbo].[UpdateDetailsCatalog]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CatalogId", catalog.CatalogId);
                cmd.Parameters.AddWithValue("@Specie", catalog.Specie);
                cmd.Parameters.AddWithValue("@CommonNoun", catalog.CommonNoun);
                cmd.Parameters.AddWithValue("@Description", catalog.Description);
                cmd.Parameters.AddWithValue("@Habits", catalog.Habits);
                cmd.Parameters.AddWithValue("@Habitat", catalog.Habitat);
                cmd.Parameters.AddWithValue("@Reproduction", catalog.Reproduction);
                cmd.Parameters.AddWithValue("@Distribution", catalog.Distribution);
                cmd.Parameters.AddWithValue("@Feeding", catalog.Feeding);
                cmd.Parameters.AddWithValue("@Category", catalog.Category);
                cmd.Parameters.AddWithValue("@AdminId", userId);
                cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar) { Direction = ParameterDirection.Output, Size = -1 });
                cmd.Parameters.Add(new SqlParameter("@IsError", SqlDbType.Bit) { Direction = ParameterDirection.Output });

                await conn.OpenAsync();
                await cmd.ExecuteReaderAsync();

                result.Message = cmd.Parameters["@Message"].Value.ToString();
                bool.TryParse(cmd.Parameters["@IsError"].Value.ToString(), out bool isError);
                result.Error = isError;

                await conn.CloseAsync();
            }
            return result;
        }

        public async Task<MessageResponse> ProcessRecord(int recordId, string status, string userId)
        {
            var result = new MessageResponse();
            Enum.TryParse<RecordStatus>(status, ignoreCase: true, out var statusout);
            int statusValue = (int)statusout;
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("[dbo].[SP_Process_Record]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@RecordId", recordId);
                cmd.Parameters.AddWithValue("@Status", statusout);
                cmd.Parameters.AddWithValue("@AdminId", userId);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Message = reader["Message"].ToString();
                    bool.TryParse(reader["IsError"].ToString(), out bool isError);
                    result.Error = isError;
                }

                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public async Task<UserEmailData> GetEmailData(int recordId)
        {
            var result = new UserEmailData();
            var sql = ResourceHelper.GetResource("GetEmailData");
            using (var con = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@RecordId", recordId);

                await con.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Email = reader["Email"].ToString();
                    result.UserName = reader["FullName"].ToString();
                }
                await con.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public async Task<MessageResponse> InsertNewCatalog(CatalogRequest model, string userId)
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
                cmd.Parameters.AddWithValue("@ImageGuid", model.Image);
                cmd.Parameters.AddWithValue("@AdminId", Convert.ToInt32(userId));

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

        public async Task<string> GetCatalogImage(int catalogId)
        {
            var result = "";
            var sql = ResourceHelper.GetResource("GetCatalogImage");

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CatalogId", catalogId);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result = reader["ImageGuid"].ToString();
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public async Task<UsersDtoResponse> GetAllUsers(int pageNumber, int pageSize)
        {
            var result = new UsersDtoResponse();
            var sql = ResourceHelper.GetResource("GetAllUsers");

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@PageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.AddWithValue("@PageSize", SqlDbType.Int).Value = pageSize;

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var user = new UserModelDto();
                    user.UserName = reader["UserName"].ToString();
                    user.Name = reader["Name"].ToString();
                    user.LastName = reader["LastName"].ToString();
                    user.Gender = reader["Gender"].ToString();
                    user.Locality = reader["Locality"].ToString();
                    int.TryParse(reader["Age"].ToString(), out int age);
                    user.Age = age;
                    user.Email = reader["Email"].ToString();

                    result.Users.Add(user);
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
            }
            return result;
        }
    }
    #endregion
}
