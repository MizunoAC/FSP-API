using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

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
        public readonly string _rootPath;

        public UpdateCatalogImgCommandHandller(IAdminRepository adminRepository, IConfiguration config)
        {
            _adminRepository = adminRepository;
            _rootPath = config["ImageSettings:RootPathCatalog"];
        }

        public async Task<MessageResponse> Handle(UpdateCatalogImgCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminRepository.GetCatalogImage(request.CatalogImg.CatalogId);
            if (result == null)
            {
                byte[] imageBytes = Convert.FromBase64String(request.CatalogImg.Image);
                string fileName = $"{result}{".png"}";

                string fullPath = Path.Combine(_rootPath, fileName);

                await File.WriteAllBytesAsync(fullPath, imageBytes, cancellationToken);

                return new MessageResponse
                {
                    Error = false,
                    Message = "Image updated successfully"
                };
            }
            return new MessageResponse
            {
                Error = true,
                Message = "Catalog not found"
            };
        }

    }
}