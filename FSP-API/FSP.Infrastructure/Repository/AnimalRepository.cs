using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Domain.Models.DTO;

namespace FSP.Infrastructure.Repository
{
    public class AnimalsRepository : IAnimalRepository
    {
        private readonly SqlConnection _conn;

        // El constructor recibe la conexión inyectada
        public AnimalsRepository(SqlConnection connection)
        {
            _conn = connection ?? throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
        }
        public async Task<MessageResponse> RegisterNewRecord(AnimalRecordRequest model, string userId)
        {

            var result = new MessageResponse();
            using (var cmd = new SqlCommand())
             {
                cmd.Connection = _conn;
                cmd.CommandText = "[dbo].[InsertNewAnimalRecord]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CommonNoun", model.CommonNoun);
                cmd.Parameters.AddWithValue("@AnimalState", model.AnimalState);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@img", model.img);
                cmd.Parameters.AddWithValue("@Location", model.Location);

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