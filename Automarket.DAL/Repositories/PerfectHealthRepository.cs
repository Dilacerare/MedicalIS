using Automarket.DAL.Interfaces;
using Automarket.Domain.Entity;

namespace Automarket.DAL.Repositories;

public class PerfectHealthRepository : IBaseRepository<PerfectHealth>
{
    private readonly ApplicationDbContext _db;

    public PerfectHealthRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task Create(PerfectHealth entity)
    {
        await _db.Healths.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public IQueryable<PerfectHealth> GetAll()
    {
        return _db.Healths;
    }

    public async Task Delete(PerfectHealth entity)
    {
        _db.Healths.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<PerfectHealth> Update(PerfectHealth entity)
    {
        _db.Healths.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }
}