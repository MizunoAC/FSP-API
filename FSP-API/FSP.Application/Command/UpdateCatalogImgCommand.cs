using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using System.Net;
using System.Numerics;

namespace FSP.Application.Command 
{
    public class UpdateCatalogImgCommand : IRequest<MessageResponse>
    {
        public CatalogImgDto CatalogImg { get; set; }
        public UpdateCatalogImgCommand(CatalogImgDto catalogImgDto)   
        { 
            CatalogImg = catalogImgDto;
        }
    }

    public class UpdateCatalogImgCommandHandller : IRequestHandler<UpdateCatalogImgCommand, MessageResponse>
    {
        private readonly IAdminRepository _adminRepository;

        public UpdateCatalogImgCommandHandller(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<MessageResponse> Handle(UpdateCatalogImgCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminRepository.UpdateCatalogImg(request.CatalogImg);
            if (result.Error)
            {
                throw new HttpRequestException(result.Message, null, HttpStatusCode.BadRequest);
            }
            return result;
        }
    }
}