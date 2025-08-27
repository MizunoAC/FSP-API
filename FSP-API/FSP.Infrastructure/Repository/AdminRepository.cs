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
        public async Task<MessageResponse> UpdateCatalog(CatalogRequestDto catalog)
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
                cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar) { Direction = ParameterDirection.Output, Size = -1});
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
                Credentials = new NetworkCredential(_config["MailSettings:Mail"], _config["MailSettings:Password"]),
                EnableSsl = true
            };
            smtp.Send(mailMessage);
        }

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

        public async Task<MessageResponse> UpdateCatalogImg(CatalogImgDto catalogImg)
        {
            var result = new MessageResponse();
            string base64String = catalogImg.Image;
            byte[] imagenBytes = Convert.FromBase64String(base64String);

            using (SqlConnection conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("[dbo].[UpdateCatalogImg]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CatalogId", catalogImg.CatalogId);
                cmd.Parameters.AddWithValue("@Image", imagenBytes);
                cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar) { Direction = ParameterDirection.Output, Size = -1 });
                cmd.Parameters.Add(new SqlParameter("@Error", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                await conn.OpenAsync();
                await cmd.ExecuteReaderAsync();
                result.Message = cmd.Parameters["@Message"].Value.ToString();
                bool.TryParse(cmd.Parameters["@Error"].Value.ToString(), out bool isError);
                result.Error = isError;
                await conn.CloseAsync();
            }
            return result;
        }
        #endregion
    }
}