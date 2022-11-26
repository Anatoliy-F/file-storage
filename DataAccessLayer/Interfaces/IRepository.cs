namespace DataAccessLayer.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity, new()
    {
        //Task<IEnumerable<TEntity>> GetPageAsync(int pageSize, int pageIndex);
        IQueryable<TEntity> GetAll();
        //Task<IEnumerable<TEntity>> GetPageFilteredAndOrdered(QueryOptionsModel query);

        Task<TEntity?> GetByIdAsync(Guid id);

        Task AddAsync(TEntity entity);

        void Delete(TEntity entity);

        Task DeleteByIdAsync(Guid id);

        void Update(TEntity entity);
    }
}
