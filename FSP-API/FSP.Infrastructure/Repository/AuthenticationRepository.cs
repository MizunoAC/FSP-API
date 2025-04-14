using Microsoft.Data.SqlClient;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using FSP.Domain.Models.Wrapper;

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

        public async Task<string> Authentication(UserAuthentication user)
        {
            using (SqlConnection conn = new SqlConnection(_con))
            using (var cmd = new SqlCommand("[dbo].[ValidateUserLogin]", conn))
            {
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

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();

                if ((bool)validate.Value)
                {
                    return this.TokenGenerationRS(userId.Value.ToString());
                }
                else
                {
                    return "An error occurred while authenticating.";
                }
            }
        }

        public string TokenGenerationRS(string User)
        {
            var rsa = RSA.Create();
            string path = _config["Jwt:PrivateKeyPath"];
            string privateKey = File.ReadAllText(path);
            rsa.ImportFromPem(privateKey);

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, User),
                new Claim(ClaimTypes.Role, "Admin")
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
    }
}