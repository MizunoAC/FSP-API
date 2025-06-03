using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FSP.Application.Query
{
    public class GetResetCodeQuery:IRequest<int>
    {
        public string Email { get; set; }

        public GetResetCodeQuery(string email)
        {
            Email = email;
        }

        public class GetResetCodeQueryHandler : IRequestHandler<GetResetCodeQuery, int>
        {
            private readonly IAuthenticationRepository _Repository;
            public GetResetCodeQueryHandler(IAuthenticationRepository Repository)
            {
                _Repository = Repository;
            }
            public async Task<int> Handle(GetResetCodeQuery request, CancellationToken cancellationToken)
            {
             return await  _Repository.GenerateResetCode(request.Email);
            }
        }
    }
}
