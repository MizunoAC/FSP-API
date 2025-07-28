using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class VerifyCodeQuery : IRequest<MessageResponse>
    {
        public VerifyCodeQuery(string email, int code) 
    { 
            Email = email;
            Code = code;
    }
        public string Email { get; set; }
        public int Code { get; set; }   
    }

    public class VerifyCodeQueryHandler(IAuthenticationRepository _Repository) : IRequestHandler<VerifyCodeQuery, MessageResponse>
    {
        public async Task<MessageResponse> Handle(VerifyCodeQuery request, CancellationToken cancellation)
        {
            return await _Repository.VerifyCode(request.Email, request.Code);
        }
    }
}