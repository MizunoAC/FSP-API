﻿using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using FSP.Domain.Models.Wrapper;
using FSP.Domain.Enums;
using RazorEngineCore;
using System.Net.Mail;
using System.Net;

namespace FSP.Infrastructure.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IConfiguration _config;
        private readonly string? _con;

        public AuthenticationRepository(IConfiguration config, DbConnectionConfig con)
        {
            _config = config;
            _con = con.ConnectionString;

        }

        public async Task<MessageResponse> Authentication(UserAuthentication user)
        {
            MessageResponse result = new MessageResponse();
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[ValidateUserLogintest]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserDomain", user.UserName);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    bool.TryParse(reader["Validate"].ToString(), out bool validate);

                    if (validate)
                    {
                        int.TryParse(reader["UserId"].ToString(), out int userId);
                        UserType userType = (UserType)reader["UserType"];
                        result.Message = this.TokenGenerationRS(userId.ToString(), userType);
                    }
                    else
                    {
                        result.Message = reader["Message"].ToString();
                    }
                    result.Error = validate;
                }
                await conn.CloseAsync();
            }
            return result;
        }

        public string TokenGenerationRS(string User, UserType userType)
        {
            var rsa = RSA.Create();
            string path = _config["Jwt:PrivateKeyPath"];
            string privateKey = File.ReadAllText(path);
            rsa.ImportFromPem(privateKey);

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, User),
                new Claim(ClaimTypes.Role,userType.ToString() )
           };

            var token = new JwtSecurityToken
            (
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<int> GenerateResetCode(string email)
        {
            DotNetEnv.Env.Load();
            string Key = Environment.GetEnvironmentVariable("sqlkey");
            var result = new UserEmailData();

            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[GenerateChagePasswordCode]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Key", Key);
                cmd.Parameters.AddWithValue("@Email", email);
                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    result.Status = reader["Code"].ToString();
                    result.UserName = reader["UserName"].ToString();
                }
                await conn.CloseAsync();
            }
            if (result != null && !result.Status.Contains("The Email"))
            {
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Email_Notification_Reset_Password.html");
                string templateContent = System.IO.File.ReadAllText(templatePath);
                IRazorEngine razorEngine = new RazorEngine();
                IRazorEngineCompiledTemplate template = razorEngine.Compile(templateContent);
                string emailBody = template.Run(result);

                var subject = "Restablecer Contraseña";
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["MailSettings:Mail"], "Fauna Silvestre"),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = emailBody
                };

                mailMessage.To.Add(email);

                var smtp = new SmtpClient("smtp.zoho.com", 587)
                {
                    Credentials = new NetworkCredential(_config["MailSettings:Mail"], _config["MailSettings:Password"]),
                    EnableSsl = true
                };
                smtp.Send(mailMessage);
            }

            return 25;
        }
    }
}