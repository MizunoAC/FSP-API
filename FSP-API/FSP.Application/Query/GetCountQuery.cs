using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetCountQuery : IRequest<CountDto>
    {
    }
    
    public class GetCountsQueryHandler : IRequestHandler<GetCountQuery, CountDto>
        
    {
        public readonly IUserRepository _userRepository;
        public GetCountsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CountDto> Handle(GetCountQuery request, CancellationToken cancellationToken) 
        {
            return await _userRepository.Count();
        }
    }
}