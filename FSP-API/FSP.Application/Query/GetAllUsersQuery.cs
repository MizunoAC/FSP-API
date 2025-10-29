using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetAllUsersQuery : IRequest<UsersDtoResponse>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public bool IsActive { get; set; }
        public GetAllUsersQuery(int page, int size, bool isActive)
        {
            Page = page;
            Size = size;
            IsActive = isActive;
        }
    }

    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, UsersDtoResponse>
    {
        private readonly IAdminRepository _repository;
        public GetAllUsersHandler(IAdminRepository repository)
        {
            _repository = repository;
        }
        public async Task<UsersDtoResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _repository.GetAllUsers(request.Page, request.Size, request.IsActive);
            return users;
        }
    }
}