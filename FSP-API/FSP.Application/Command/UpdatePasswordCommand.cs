using FSP.Domain.Models.DTO;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;
using Sprache;

namespace FSP.Application.Command
{
    public class UpdatePasswordCommand : IRequest<string>
    {
        public ResetPasswordDTO ResetPassword { get; set; }
        public UpdatePasswordCommand(ResetPasswordDTO resetPasswordDTO)
        {
            ResetPassword = resetPasswordDTO; 
        }
    }

    public class UpdatePasswordCommandHandler(IAuthenticationRepository authenticationRepository) : IRequestHandler<UpdatePasswordCommand, string> 
    {
        public async Task<string> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
           return await authenticationRepository.ResetPassword(request.ResetPassword);
        }
    }
}