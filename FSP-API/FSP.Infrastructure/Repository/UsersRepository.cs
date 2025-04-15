using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Domain.Models.DTO;
using FSP.Domain.Models.Wrapper;
using FSP.Domain.Helpers;

namespace FSP.Infrastructure.Repository
{
    public class UsersRepository : IUserRepository
    {
        private readonly string? _con;

        public UsersRepository(DbConnectionConfig con)
        {
            _con = con.ConnectionString;
        }
        public async Task<MessageResponse> RegisterNewUser(UserModelRequest model)
        {

            var result = new MessageResponse();
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[InsertNewUserApp]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@Gender", model.Gender);
                cmd.Parameters.AddWithValue("@Age", model.Age);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Locality", model.Locality);

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

        public async Task<UserModelDto> GetUserByID(string UserId)
        {
            var user = new UserModelDto();
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[GetUserById]", conn)) 
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", UserId);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) 
                {
                    user.UserName = reader["UserName"].ToString();
                    user.Name = reader["Name"].ToString();
                    user.LastName = reader["LastName"].ToString();
                    user.Gender = reader["Gender"].ToString();
                    user.Locality = reader["Locality"].ToString();
                    int.TryParse(reader["Age"].ToString(), out int age);
                    user.Age = age;
                    user.Email = reader["Email"].ToString();
                }

                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return user;
        }

        public async Task<MessageResponse> DeleteUser(int UserId)
        {
            var result = new MessageResponse();
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[DeleteUserApp]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", UserId);

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

        public async Task<CountDto> Count()
        {
            var result = new CountDto();
            var sql = ResourceHelper.GetResource("GetCount");
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int.TryParse(reader["Records"].ToString(), out int records);
                    result.Records = records;
                    int.TryParse(reader["Users"].ToString(), out int users);
                    result.Users = users;

                }
            }
                return result;
        }
    }
}