using FSP_API.Context;
using FSP_API.Modelos;
using FSP_API.ModelosDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FSP_API.Excepcionescontroladas;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FSP_API.Servicios
{
    public class LoginServicio : ILoginServicio

    {
        private readonly ContexDb _contextoDb;
        private readonly IConfiguration _config;



        public LoginServicio(ContexDb contextoDb, IConfiguration config) {

            _contextoDb = contextoDb;
            _config = config;
        }

        public async Task<ModeloUsuario> Autentificacion(ModeloLoginDTO modeloLoginDTO)
        {
            
                var Usuario = await _contextoDb.ModeloUsuarios.Where(c => c.NombreUsuario == modeloLoginDTO.NombreUsuario).FirstOrDefaultAsync();

                if (Usuario == null)
                {

                throw new Excepcion3();
                }

                else
                {


                    var ContrasenaCorrecta = BCrypt.Net.BCrypt.Verify(modeloLoginDTO.Contraseña, Usuario.Contraseña);
                    //var ContrasenaCorrecta = Usuario.Contraseña.ToLower().Equals(modeloLoginDTO.Contraseña);


                    if (ContrasenaCorrecta == true)
                    {

                        return Usuario;

                    }


                    else
                    {
                        throw new Excepcion4();
                    }

                }
            
        }


        public  string GereneraciondelToken(ModeloUsuario usuario){

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.GivenName, usuario.Nombre),
                new Claim(ClaimTypes.Surname, usuario.ApellidoPaterno),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario),
                new Claim("Id", usuario.ModeloUsuarioId.ToString())
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
