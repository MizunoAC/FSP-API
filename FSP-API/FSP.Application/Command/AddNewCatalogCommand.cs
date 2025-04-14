using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Command
{
    public class AddNewCatalogCommand : IRequest<MessageResponse>
    {
        public CatalogRequest Model { get; set; }

        public AddNewCatalogCommand(CatalogRequest model)
        {
            Model = model;
        }

        public class AddNewCatalogCommandHandler : IRequestHandler<AddNewCatalogCommand, MessageResponse>
        {
            private readonly IAnimalRepository _animalRepository;

            public AddNewCatalogCommandHandler(IAnimalRepository animalRepository)
            {
                _animalRepository = animalRepository;
            }

            public async Task<MessageResponse> Handle(AddNewCatalogCommand request, CancellationToken cancellationToken) 
            {
                return await _animalRepository.InsertNewCatalog(request.Model);
            }
        }

    }
}