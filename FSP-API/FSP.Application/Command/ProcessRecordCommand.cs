using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Command
{
    public class ProcessRecordCommand : IRequest<MessageResponse>
    {
        public int RecordId { get; set; }
        public ´string Status { get; set; }
        public ProcessRecordCommand(int recordId, string status)
        {
            RecordId = recordId;
            Status = status;
        }
    }

    public class UpdateRecordStatusHandler : IRequestHandler<ProcessRecordCommand, MessageResponse>
    {
        private readonly IAnimalRepository _animalRepository;

        public UpdateRecordStatusHandler(IAnimalRepository animalRepository )
        {
            _animalRepository = animalRepository;
        }

        public async Task<MessageResponse> Handle(ProcessRecordCommand request, CancellationToken cancellationToken)
        {
            return await _animalRepository.ProcessRecord(request.RecordId, request.Status);
        }
    }
}
