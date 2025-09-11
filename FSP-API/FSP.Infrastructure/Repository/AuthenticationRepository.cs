using Microsoft.Data.SqlClient;
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
using FSP.Domain.Models.DTO;
using Azure;
using System.Data;
using FSP.Domain.Helpers;
using Azure.Core;

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

        public async Task<TokenResult> Authentication(UserAuthentication user)
        {
            TokenResult result = new TokenResult();
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
                        var tokens = this.TokenGenerationRS(userId.ToString(), userType);
                        result.AccessToken = tokens.AccessToken;
                        result.RefreshToken = tokens.RefreshToken;
                    }
                    else
                    {
                        result.Message = reader["Message"].ToString();
                    }
                    result.Error = validate;

                }
                await conn.CloseAsync();
                await reader.DisposeAsync();
            }
            return result;
        }

        public TokenResult TokenGenerationRS(string User, UserType userType)
        {
            var rsa = RSA.Create();
            var keyPath = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY");
            var privateKey = File.ReadAllText(keyPath);

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

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, User),
                new Claim("typ", "refresh"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );
            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            if (refreshTokenString != null)
            {
                SaveRefreshToken(User, refreshTokenString, DateTime.Now.AddDays(7));
            }

            return new TokenResult
            {
                RefreshToken = refreshTokenString,
                AccessToken = accessToken
            };
        }

        public async void SaveRefreshToken(string userId, string token, DateTime expiresAt)
        {
            var sql = ResourceHelper.GetResource("");
            using (var cnn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[InsertRefreshToken]", cnn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);
                await cnn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await cnn.CloseAsync();
            }
        }

        public string TokenGenerationRSPasswordReset(string User)
        {
            var rsa = RSA.Create();
            var keyPath = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY");
            var privateKey = File.ReadAllText(keyPath);
            rsa.ImportFromPem(privateKey);

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, User),
                new Claim("purpose", "password_reset")
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

        public async Task<string> GenerateResetCode(string email)
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
                await reader.DisposeAsync();
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
                return "code sent successfully";
            }
            return result.Status;
        }

        public async Task<string> ResetPassword(ResetPasswordDTO reset)
        {
            MessageResponse result = new MessageResponse();
            var response = "";
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[UpdatePasswordV2]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Email", reset.Email);
                cmd.Parameters.AddWithValue("@Password", reset.Password);

                SqlParameter message = new SqlParameter("@Message", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Output,
                    Size = -1
                };
                cmd.Parameters.Add(message);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                response = message.Value?.ToString();
                await conn.CloseAsync();
            }
            return response;
        }
        public async Task<MessageResponse> VerifyCode(string email, int code)
        {
            MessageResponse result = new MessageResponse();
            SqlParameter userId = new SqlParameter();
            var response = "";
            DotNetEnv.Env.Load();
            string Key = Environment.GetEnvironmentVariable("sqlkey");
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[VerifyCode]", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithValue("@Key", Key);
                cmd.Parameters.AddWithValue("@Email", email);

                SqlParameter message = new SqlParameter("@Message", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Output,
                    Size = -1
                };
                cmd.Parameters.Add(message);
                SqlParameter isError = new SqlParameter("@IsError", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(isError);
                userId = new SqlParameter("@UserId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(userId);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                result.Error = isError.Value != DBNull.Value && (bool)isError.Value;
                result.Message = message.Value?.ToString();
                await conn.CloseAsync();
            }
            if (userId != null && result.Error == false)
            {
                result.Message = this.TokenGenerationRSPasswordReset(userId.Value.ToString());
            }
            return result;
        }

        public TokenResult RefreshToken(string refreshToken)
        {

            var rsa = RSA.Create();
            string publicKey = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, _config["Jwt:PublicKeyPath"]));
            rsa.ImportFromPem(publicKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, validationParams, out _);

                var typeClaim = principal.Claims.FirstOrDefault(c => c.Type == "typ");
                if (typeClaim?.Value != "refresh")
                    throw new SecurityTokenException("Token is not of type refresh");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    throw new SecurityTokenException("Invalid token: no user");

                var isValid = IsValidRefreshToken(refreshToken, userId);
                if (isValid)
                {
                    UserType userType = (UserType)2;
                    return TokenGenerationRS(userId, userType);

                }
                else
                {
                    return new TokenResult
                    {
                        Error = true,
                        Message = "An error occurred while validating the refresh token."
                    };

                }
            }
            catch (Exception ex)
            {
                return new TokenResult
                {
                    Error = true,
                    Message = ex.Message
                };
            }

        }

        public bool IsValidRefreshToken(string refreshtoken, string userId)
        {
            var sql = ResourceHelper.GetResource("IsValidRefreshToken");
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Token", refreshtoken);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                var result = Convert.ToBoolean(cmd.ExecuteScalar());
                conn.Close();
                return result;
            }

        }
    }
}