using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task AddAsync(RefreshToken token);
    Task<bool> SaveChangesAsync();
}