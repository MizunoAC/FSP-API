using FSP.Domain.Models;
using FSP.Infrastructure.Repository.Contracts;
using MediatR;

namespace FSP.Application.Query
{
    public class GetTotalStatisticsQuery : IRequest<TotalStatistics>
    {
    }

    public class GetTotalStatisticsQueryHandler(IAdminRepository adminRepository) : IRequestHandler<GetTotalStatisticsQuery, TotalStatistics>
    {
        public async Task<TotalStatistics> Handle(GetTotalStatisticsQuery request, CancellationToken cancellationToken)
        {
            return await adminRepository.GetTotalStatistics();
        }
    }
}