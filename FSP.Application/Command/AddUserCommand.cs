using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.command
{
    public class AddUserCommand : IRequest<MessageResponse>
    {
        public UserModelRequest Model { get; set; }

        public AddUserCommand(UserModelRequest model)
        {
          Model = model;
        }
    }

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, MessageResponse>
    {
        private readonly IUserRepository _userRepository;

        public AddUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<MessageResponse> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.RegisterNewUser(request.Model);
        }
    }   
}