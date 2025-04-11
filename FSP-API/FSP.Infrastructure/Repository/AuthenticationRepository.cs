using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Domain.Models.DTO;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace FSP.Infrastructure.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly SqlConnection _conn;
        private readonly IConfiguration _config;

        // El constructor recibe la conexión inyectada
        public AuthenticationRepository(SqlConnection connection, IConfiguration config)
        {
            _config = config;
            _conn = connection ?? throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
        }

        public async Task<string> Authentication(UserAuthentication user)
        {
            using (var cmd = new SqlCommand()) 
            {

                cmd.Connection = _conn;
                cmd.CommandText = "[dbo].[ValidateUserLogin]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserDomain", user.UserName);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                var userId = new SqlParameter("@UserId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(userId);
                var validate = new SqlParameter("@Validate", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(validate);

                await _conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await _conn.CloseAsync();

                if ((bool)validate.Value)
                {
                    return this.TokenGenerationRS(userId.Value.ToString());
                }
                else         
                {
                    return "error";
                }

                //return (bool)validate.Value;
            }
       
        }
        public  string TokenGenerationRS(string User)
        {
            var rsa = RSA.Create();
            string path = _config["Jwt:PrivateKeyPath"];
            string privateKey = File.ReadAllText(path);
            rsa.ImportFromPem(privateKey);

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, User),
           };

            var token = new JwtSecurityToken
            (
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return  new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}