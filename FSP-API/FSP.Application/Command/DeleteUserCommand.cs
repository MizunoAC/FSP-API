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
    public class DeleteUserCommand : IRequest<MessageResponse>
    {
        public int UserId { get; set; }

        public DeleteUserCommand(int userId)
        {
          UserId = userId;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, MessageResponse>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<MessageResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.DeleteUser(request.UserId);
        }
    }   
}