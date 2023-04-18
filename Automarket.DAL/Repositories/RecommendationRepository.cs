using Automarket.DAL.Interfaces;
using Automarket.Domain.Entity;

namespace Automarket.DAL.Repositories;

public class RecommendationRepository : IBaseRepository<Recommendation>
{
    private readonly ApplicationDbContext _dbContext;

    public RecommendationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(Recommendation entity)
    {
        await _dbContext.Recommendations.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<Recommendation> GetAll()
    {
        return _dbContext.Recommendations;
    }

    public async Task Delete(Recommendation entity)
    {
        _dbContext.Recommendations.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Recommendation> Update(Recommendation entity)
    {
        _dbContext.Recommendations.Update(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }
}