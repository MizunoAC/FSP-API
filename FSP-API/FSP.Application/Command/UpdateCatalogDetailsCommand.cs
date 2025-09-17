using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using System.Net;

namespace FSP.Application.Command
{
    public class UpdateCatalogDetailsCommand : IRequest<MessageResponse>
    {
        public CatalogRequestDto Catalog { get; set; }
        public string UserId { get; set; }
        public UpdateCatalogDetailsCommand(CatalogRequestDto catalog, string userId)
        {
            Catalog = catalog;
            UserId = userId;
        }
        
    }

    public class UpdateCatalogDetailsCommandHandler : IRequestHandler<UpdateCatalogDetailsCommand, MessageResponse>
    {
        private readonly IAdminRepository _adminRepository;
        public UpdateCatalogDetailsCommandHandler(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public async Task<MessageResponse> Handle(UpdateCatalogDetailsCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminRepository.UpdateCatalog(request.Catalog, request.UserId);

            if (result.Error)
            {
                throw new HttpRequestException(result.Message, null, HttpStatusCode.BadRequest);
            }
            return result;
        }
    }
}
