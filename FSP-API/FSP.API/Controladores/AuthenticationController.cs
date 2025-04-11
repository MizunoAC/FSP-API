using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FSP_API.Context;
using FSP_API.Excepcionescontroladas;
using FSP_API.Modelos.ViewModel;
using FSP_API.ModelosDTO;
using FSP_API.Servicios;
using System.Text.Json.Nodes;
using FSP.Domain.Models;
using FSP.Application.command;
using MediatR;
using FSP.Application.Query;
using Microsoft.AspNetCore.Components.Forms;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase 
    {

        private readonly ILoginServicio _loginServicio;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IMediator _mediator;


        public AuthenticationController(ILoginServicio loginServicio, ILogger<AuthenticationController> logger, IMediator mediator)
        {
            _loginServicio = loginServicio;
            _logger = logger;
            _mediator = mediator;
        }

        //Metodo para generar iniciar sesion

        [HttpPost("LogIn")]

        public async Task <IActionResult> Login (UserAuthentication User )
        {

            try
            {
                var command = new UserAuthenticationQuery(User);
                var result = await _mediator.Send(command);
                return Ok(result);
            }

            catch(Excepcion3 ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return BadRequest("Usuario no registrado");
            }
            catch (Excepcion4 ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return  BadRequest("Contraseña incorrecta");
            }

        }


    }
}
