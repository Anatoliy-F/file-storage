namespace DataAccessLayer.Interfaces
{
    /// <summary>
    /// Represents common operations
    /// </summary>
    /// <typeparam name="TEntity">Entitis with Guid primary key</typeparam>
    public interface IRepository<TEntity> where TEntity : IEntity, new()
    {
        /// <summary>
        /// Get all entities (without related data), result should be filtered, ordered and paginated
        /// Entities tracked by context!
        /// Don't return all sequnce to client
        /// </summary>
        /// <returns>Return all entities without related data</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Get entity by id (without related data)
        /// </summary>
        /// <param name="id">Entities id</param>
        /// <returns>Entity (without related data) or null</returns>
        Task<TEntity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Save entity in storage
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Void</returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Delete entity from storage
        /// </summary>
        /// <param name="entity">Entity instance</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Update entity in storage
        /// </summary>
        /// <param name="entity">Entity instance</param>
        void Update(TEntity entity);
    }
}
