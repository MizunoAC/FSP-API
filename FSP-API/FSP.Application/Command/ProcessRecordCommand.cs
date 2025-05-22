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
        private readonly IAnimalRepository _animalRepository;

        public UpdateRecordStatusHandler(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public async Task<MessageResponse> Handle(ProcessRecordCommand request, CancellationToken cancellationToken)
        {
            Enum.TryParse<RecordStatus>(request.Status, ignoreCase: true, out var statusout);
            var result = await _animalRepository.ProcessRecord(request.RecordId, request.Status);

            if (statusout == RecordStatus.Accepted && result != null)
            {
                var emailData = await _animalRepository.GetEmailData(request.RecordId);
                emailData.Status = "Aceptado";
                _animalRepository.SendEmailNotificacion(emailData, request.Rootenv);
            }
            return result;
        }
    }
}
