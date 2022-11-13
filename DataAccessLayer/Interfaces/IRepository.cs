using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity, new()
    {
        Task<ICollection<TEntity>> GetPageAsync(int pageSize, int pageIndex);
        Task<ICollection<TEntity>> GetAllAsync();
        Task<ICollection<TEntity>> GetPageFilteredAndOrdered(int pageSize, int pageIndex,
            string? sortColumn = null, string? sortOrder = null,
            string? filterColumn = null, string? filterQuery = null);

        Task<TEntity?> FindByIdAsync(int id);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void DeleteByIdAsync(int id);
        void Update(TEntity entity);
    }
}
