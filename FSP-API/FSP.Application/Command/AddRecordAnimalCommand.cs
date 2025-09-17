using DotNetEnv;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

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
        private readonly string _rootPath;

        public AddRecordAnimalCommandHandler(IAnimalRepository animalReporsity, IConfiguration config)
        {
            _animalRepository = animalReporsity;
            _rootPath = config["ImageSettings:RootPath"];
        }
        public async Task<MessageResponse> Handle(AddRecordAnimalCommand request, CancellationToken cancellationToken)
        {
            byte[] imageBytes = Convert.FromBase64String(request.Model.img);
            Guid imageId = Guid.NewGuid();
            string fileName = $"{imageId}{".png"}";

            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);

            string fullPath = Path.Combine(_rootPath, fileName);

            request.Model.ImageGuid = imageId.ToString();
            await File.WriteAllBytesAsync(fullPath, imageBytes, cancellationToken);

            return await _animalRepository.RegisterNewRecord(request.Model, request.UserId);
        }
    }   
}