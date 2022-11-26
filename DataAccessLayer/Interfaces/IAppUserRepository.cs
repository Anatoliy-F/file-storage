using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        public Task<AppUser?> GetByIdWithRelatedAsync(Guid userId);

        public Task<int> GetReadOnlyFilesCountAsync(Guid userId);
    }
}
