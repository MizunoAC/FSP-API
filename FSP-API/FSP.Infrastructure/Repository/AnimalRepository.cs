using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Domain.Models.DTO;
using FSP.Domain.Helpers;
using System.Data;
using FSP.Domain.Models.Wrapper;
using FSP.Domain.Enums;
using RazorEngineCore;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.IO;

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
                    var binaryData = (byte[])reader["Image"];
                    var base64String = Convert.ToBase64String(binaryData);
                    var base64Image = $"data:image/jpeg;base64,{base64String}";
                    results.Add(new AnimalRecordDto
                    {
                        CommonNoun = reader["CommonNoun"].ToString(),
                        AnimalState = reader["AnimalState"].ToString(),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        img = base64Image
                    });
                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return results;
        }

        public async Task<List<AnimalRecordDto>> GetAllRecords(string recordStatus)
        {
            var results = new List<AnimalRecordDto>();
            var sql = ResourceHelper.GetResource("GetAllRecords");

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@RecordStatus", recordStatus);
                await conn.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int.TryParse(reader["RecordId"].ToString(), out int recordId);
                    results.Add(new AnimalRecordDto
                    {
                        RecordId = recordId,
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
            string base64String = model.Image;
            byte[] imagenBytes = Convert.FromBase64String(base64String);

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
                cmd.Parameters.AddWithValue("@Image", imagenBytes);

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
                    var binaryData = (byte[])reader["Image"];
                    var base64String = Convert.ToBase64String(binaryData);
                    var base64Image = $"data:image/jpeg;base64,{base64String}";
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
                        Image = base64Image,
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

        #region admin
        public async Task<MessageResponse> ProcessRecord(int recordId, string status)
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

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Message = reader["Message"].ToString();
                    bool.TryParse(reader["IsError"].ToString(), out bool isError);
                    result.Error = isError;
                }
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
            }
            return result;
        }

        public void SendEmailNotificacion(UserEmailData data, string rootenv)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Email_Notification.html");
            string templateContent = System.IO.File.ReadAllText(templatePath);
            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile(templateContent);
            string emailBody = template.Run(data);

            var subject = "Registro Aceptado";
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_config["MailSettings:Mail"], "Fauna Silvestre"),
                Subject = subject,
                IsBodyHtml = true,
                Body = emailBody
            };

            mailMessage.To.Add(data.Email);

            var smtp = new SmtpClient("smtp.zoho.com", 587)
            {
                Credentials = new NetworkCredential(_config["MailSettings:Mail"] , _config["MailSettings:Password"]),
                EnableSsl = true
            };
            smtp.Send(mailMessage);
        }
        #endregion
    }
}