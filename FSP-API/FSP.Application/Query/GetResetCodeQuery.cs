using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;
using RazorEngineCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace FSP.Application.Query
{
    public class GetResetCodeQuery:IRequest<string>
    {
        public string Email { get; set; }

        public GetResetCodeQuery(string email)
        {
            Email = email;
        }

        public class GetResetCodeQueryHandler : IRequestHandler<GetResetCodeQuery, string>
        {
            private readonly IAuthenticationRepository _Repository;
            private readonly IConfiguration _config;
            public GetResetCodeQueryHandler(IAuthenticationRepository Repository, IConfiguration config)
            {
                _Repository = Repository;
                _config = config;
            }
            public async Task<string> Handle(GetResetCodeQuery request, CancellationToken cancellationToken)
            {
             var result = await  _Repository.GenerateResetCode(request.Email);

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

                    mailMessage.To.Add(request.Email);

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
        }
    }
}
