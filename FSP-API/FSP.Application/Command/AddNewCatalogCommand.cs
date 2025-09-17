using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FSP.Application.Command
{
    public class AddNewCatalogCommand : IRequest<MessageResponse>
    {
        public CatalogRequest Model { get; set; }
        public string UserId { get; set; }

        public AddNewCatalogCommand(CatalogRequest model, string userId)
        {
            Model = model;
            UserId = userId;
        }

        public class AddNewCatalogCommandHandler : IRequestHandler<AddNewCatalogCommand, MessageResponse>
        {
            private readonly IAdminRepository _repository;
            private readonly string _rootPath;

            public AddNewCatalogCommandHandler(IAdminRepository Repository, IConfiguration config)
            {
                _repository = Repository;
                _rootPath = config["ImageSettings:RootPathCatalog"];
            }

            public async Task<MessageResponse> Handle(AddNewCatalogCommand request, CancellationToken cancellationToken) 
            {
                byte[] imageBytes = Convert.FromBase64String(request.Model.Image );
                Guid imageId = Guid.NewGuid();


                string fileName = $"{imageId}{".png"}";

                if (!Directory.Exists(_rootPath))
                    Directory.CreateDirectory(_rootPath);

                string fullPath = Path.Combine(_rootPath, fileName);
                request.Model.Image = imageId.ToString();
                await File.WriteAllBytesAsync(fullPath, imageBytes, cancellationToken);

                return await _repository.InsertNewCatalog(request.Model, request.UserId);
            }
        }

    }
}