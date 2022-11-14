using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity, new()
    {
        Task<ICollection<TEntity>> GetPageAsync(int pageSize, int pageIndex);
        Task<ICollection<TEntity>> GetAllAsync();
        Task<ICollection<TEntity>> GetPageFilteredAndOrdered(QueryOptionsModel query);

        Task<TEntity?> FindByIdAsync(Guid id);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void DeleteByIdAsync(Guid id);
        void Update(TEntity entity);
    }
}
