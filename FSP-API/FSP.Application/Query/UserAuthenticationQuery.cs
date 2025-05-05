using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class UserAuthenticationQuery : IRequest<string>
    {
        public UserAuthentication User { get; set; }

        public UserAuthenticationQuery(UserAuthentication user)
        {
            User = user;
        }
    }

    public class UserAuthenticationQueryHandler : IRequestHandler<UserAuthenticationQuery, string>
    {
        private readonly IAuthenticationRepository _Repository;
        public UserAuthenticationQueryHandler(IAuthenticationRepository Repository)
        {
            _Repository = Repository;
        }
        public async Task<string> Handle(UserAuthenticationQuery request, CancellationToken cancellationToken)
        {
            var result = await _Repository.Authentication(request.User);

            if (result == "Error") 
            {
                throw new HttpRequestException("Invalid credentials", null, HttpStatusCode.Unauthorized);
            }
            return result;
        }
    }
}

