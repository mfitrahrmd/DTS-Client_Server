using API.Models;

namespace API.Repositories.Contracts;

public interface IRoleRepository : IBaseRepository<int, TbMRole>
{
    public Task<TbMRole> FindOneOrInsertByName(string name);
}