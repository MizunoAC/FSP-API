using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetUserByIDQuery : IRequest<UserModelDto>
    {
        public string UserId { get; set; }

        public GetUserByIDQuery(string userId)
        {
            UserId = userId;
        }
    }

    public class GetUserByIDQueryHandler : IRequestHandler<GetUserByIDQuery ,UserModelDto> 
    {
        private readonly IUserRepository _userRepository;
        public GetUserByIDQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserModelDto> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetUserByID(request.UserId);
        }
    }
}
