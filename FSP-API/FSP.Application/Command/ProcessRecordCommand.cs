using FSP.Domain.Enums;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;
using RazorEngineCore;
using System.Net;
using System.Net.Mail;

namespace FSP.Application.Command
{
    public class ProcessRecordCommand : IRequest<MessageResponse>
    {
        public int RecordId { get; set; }
        public string Status { get; set; }
        public string Rootenv { get; set; }
        public string UserId { get; set; }

        public ProcessRecordCommand(int recordId, string status, string rootenv, string userId)
        {
            RecordId = recordId;
            Status = status;
            Rootenv = rootenv;
            UserId = userId;
        }
    }

    public class UpdateRecordStatusHandler : IRequestHandler<ProcessRecordCommand, MessageResponse>
    {
        private readonly IAdminRepository _repository;
        public readonly IConfiguration _config;

        public UpdateRecordStatusHandler(IAdminRepository Repository, IConfiguration config)
        {
            _repository = Repository;
            _config = config;
        }

        public async Task<MessageResponse> Handle(ProcessRecordCommand request, CancellationToken cancellationToken)
        {
            Enum.TryParse<RecordStatus>(request.Status, ignoreCase: true, out var statusout);
            var result = await _repository.ProcessRecord(request.RecordId, request.Status, request.UserId);

            if (statusout == RecordStatus.Accepted && result != null)
            {
                var emailData = await _repository.GetEmailData(request.RecordId);
                emailData.Status = "Aceptado";

                var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Email_Notification.html");
                string templateContent = System.IO.File.ReadAllText(templatePath);
                IRazorEngine razorEngine = new RazorEngine();
                IRazorEngineCompiledTemplate template = razorEngine.Compile(templateContent);
                string emailBody = template.Run(emailData);

                var subject = "Registro Aceptado";
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["MailSettings:Mail"], "Fauna Silvestre"),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = emailBody
                };

                mailMessage.To.Add(emailData.Email);

                var smtp = new SmtpClient("smtp.zoho.com", 587)
                {
                    Credentials = new NetworkCredential(_config["MailSettings:Mail"], _config["MailSettings:Password"]),
                    EnableSsl = true
                };
                smtp.Send(mailMessage);
            }
            return result;
        }
    }
}
