using API.Models;
using API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class UniversityRepository<TContext> : CoreRepository<int, TbMUniversity, TContext>, IUniversityRepository
    where TContext : DbContext
{
    public UniversityRepository(TContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TbMUniversity>> FindManyContainsNameAsync(string name)
    {
        return _dbSet.Where(u => u.Name.Contains(name));
    }

    public Task<TbMUniversity?> FindOneByNameAsync(string name)
    {
        var foundUniversity = _dbSet.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(u => u.Name.Equals(name));

        return foundUniversity;
    }
    
    public async Task<TbMUniversity> FindOneOrInsertByName(string name)
    {
        var foundUniversity = await _dbSet.FirstOrDefaultAsync(u => u.Name.Equals(name));

        var insertedUniversity = new TbMUniversity()
        {
            Name = name
        };

        if (foundUniversity is null)
        {
            WithTx(() =>
            {
                _dbSet.Add(insertedUniversity);
            });

            return insertedUniversity;
        }

        return foundUniversity;
    }

}