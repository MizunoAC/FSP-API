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
            private readonly IAdminRepository _repository;

            public AddNewCatalogCommandHandler(IAdminRepository Repository)
            {
                _repository = Repository;
            }

            public async Task<MessageResponse> Handle(AddNewCatalogCommand request, CancellationToken cancellationToken) 
            {
                return await _repository.InsertNewCatalog(request.Model);
            }
        }

    }
}