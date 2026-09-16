using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface IStudyService
{
    Task<DashboardDto> GetDashboardAsync(long userId);
}