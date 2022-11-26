using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IAppFileDataRepository : IRepository<AppFileData>
    {
        //public Task<ICollection<AppFileData>> GetFilteredSortedPageByUserAsync(Guid userId, QueryOptionsModel query);

        public IQueryable<AppFileData> GetAllNoTraking();

        public Task<int> GetUserFilesCountAsync(Guid userId);

        public Task<AppFileData?> GetByIdWithContentAsync(Guid id);

        public Task<int> GetFilesCountAsync();

        //public Task<ICollection<AppFileData>> GetFilteredSortedSharedWithUserAsync(Guid userId, QueryOptionsModel query);

        //public Task<ICollection<AppFileData>> GetFilteredSortedWithUserDataAsync(QueryOptionsModel query);
    }
}
