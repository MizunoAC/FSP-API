using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository
{
    public class UsersRepository : IUserRepository
    {
        private readonly SqlConnection _conn;

        // El constructor recibe la conexión inyectada
        public UsersRepository(SqlConnection connection)
        {
            _conn = connection ?? throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
        }
        public async Task<MessageResponse> RegisterNewUser(UserModelRequest model)
        {

            var result = new MessageResponse();
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandText = "[dbo].[InsertNewUserApp]";
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

                await _conn.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Message = reader["Message"].ToString();
                    bool.TryParse(reader["IsError"].ToString(), out bool error);
                    result.Error = error;
                }
                await _conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public async Task<UserModelDto> GetUserByID(string UserId)
        {
            var user = new UserModelDto();

            using (var cmd = new SqlCommand()) 
            {

                cmd.Connection = _conn;
                cmd.CommandText = "[dbo].[GetUserById]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", UserId);

                await _conn.OpenAsync();
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

                await _conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return user;
        }

        public async Task<MessageResponse> DeleteUser(int UserId)
        {
            var result = new MessageResponse();
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandText = "[dbo].[DeleteUserApp]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", UserId);

                await _conn.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Message = reader["Message"].ToString();
                    bool.TryParse(reader["IsError"].ToString(), out bool error);
                    result.Error = error;
                }
                await _conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }
    }
}