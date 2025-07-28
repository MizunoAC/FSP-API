using FSP.Domain.Enums;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Command
{
    public class ProcessRecordCommand : IRequest<MessageResponse>
    {
        public int RecordId { get; set; }
        public string Status { get; set; }
        public string Rootenv { get; set; }
        public ProcessRecordCommand(int recordId, string status, string rootenv)
        {
            RecordId = recordId;
            Status = status;
            Rootenv = rootenv;
        }
    }

    public class UpdateRecordStatusHandler : IRequestHandler<ProcessRecordCommand, MessageResponse>
    {
        private readonly IAdminRepository _repository;

        public UpdateRecordStatusHandler(IAdminRepository Repository)
        {
            _repository = Repository;
        }

        public async Task<MessageResponse> Handle(ProcessRecordCommand request, CancellationToken cancellationToken)
        {
            Enum.TryParse<RecordStatus>(request.Status, ignoreCase: true, out var statusout);
            var result = await _repository.ProcessRecord(request.RecordId, request.Status);

            if (statusout == RecordStatus.Accepted && result != null)
            {
                var emailData = await _repository.GetEmailData(request.RecordId);
                emailData.Status = "Aceptado";
                _repository.SendEmailNotificacion(emailData, request.Rootenv);
            }
            return result;
        }
    }
}
