using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.command
{
    public class AddRecordAnimalCommand : IRequest<MessageResponse>
    {
        public AnimalRecordRequest Model { get; set; }
        public string UserId { get; set; } 

        public AddRecordAnimalCommand(AnimalRecordRequest model, string userId)
        {
          Model = model;
         UserId = userId;
        }
    }

    public class AddRecordAnimalCommandHandler : IRequestHandler<AddRecordAnimalCommand, MessageResponse>
    {
        private readonly IAnimalRepository _animalRepository;

        public AddRecordAnimalCommandHandler(IAnimalRepository animalReporsity)
        {
            _animalRepository = animalReporsity;
        }
        public async Task<MessageResponse> Handle(AddRecordAnimalCommand request, CancellationToken cancellationToken)
        {
            return await _animalRepository.RegisterNewRecord(request.Model, request.UserId);
        }
    }   
}