using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Dynamic.Core;
using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories
{
    public abstract class BaseRepository<T> : IDisposable, IRepository<T> where T : class, IEntity, new()
    {
        private readonly bool _disposeContext;
        public DbSet<T> Table { get; }
        public AppDbContext Context { get; }

        protected BaseRepository(AppDbContext context)
        {
            Context = context;
            Table = Context.Set<T>();
            _disposeContext = false;
        }

        protected BaseRepository(DbContextOptions<AppDbContext> options) : this(new AppDbContext(options))
        {
            _disposeContext = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if(disposing && _disposeContext)
            {
                Context.Dispose();
            }
            _isDisposed = true;
        }

        ~BaseRepository()
        {
            Dispose(false);
        }

        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (CustomException)
            {
                //TODO: already logged - should handle
                throw;
            }
            catch (Exception ex)
            {
                //TODO: log and handle
                throw new CustomException("An error occurred updating the database", ex);
            }
        }

        public virtual async Task<ICollection<T>> GetPageAsync(int pageSize, int pageIndex)
        {
            return await Table.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<ICollection<T>> GetAllAsync() => await Table.ToListAsync();

        public virtual async Task<T?> FindByIdAsync(int id) => await Table.FindAsync(id);

        public async Task AddAsync(T entity) => await Table.AddAsync(entity);

        public void Delete(T entity) => Table.Remove(entity);


        public virtual async void DeleteByIdAsync(int id)
        {
            await Task.Run(() =>
            {
                if (Table.Local.Any(e => e.Id == id))
                {
                    Table.Remove(Table.Local.First(e => e.Id == id));
                }
                else
                {
                    Context.Entry(new T { Id = id }).State = EntityState.Deleted;
                }
            });
        }

        public virtual void Update(T entity) => Table.Update(entity);

        public async Task<ICollection<T>> GetPageFilteredAndOrdered(QueryOptionsModel query)
        {
            return await TakePageFilteredAndOrdered(Table, query);
        }


        // <summary>
        /// Pages, sorts and/or filters a IQueryable source
        /// </summary>
        /// <param name="source">An IQueryable source of generic type</param>
        /// <param name="pageIndex">Zero-based current page index (0 = first page)</param>
        /// <param name="pageSize">The actual size of each page</param>
        /// <param name="sortColumn">The sorting column name</param>
        /// <param name="sortOrder">The sorting order ("ASC" or "DESC")</param>
        /// <param name="filterColumn">The filtering column name</param>
        /// <param name="filterQuery">The filtering auery (value to lookup)</param>
        /// <returns>A object containing the IQueryable paged/sorted/filtered result
        /// and all the relevant paging/sorting/filtering navigation info</returns>
        protected async Task<ICollection<T>> TakePageFilteredAndOrdered(IQueryable<T> source, QueryOptionsModel query)
        {
            if(!string.IsNullOrEmpty(query.filterColumn) && !string.IsNullOrEmpty(query.filterQuery) && IsValidProperty(query.filterColumn))
            {
                source = source.Where(string.Format("{0}.StartsWith(@0)", query.filterColumn), query.filterQuery);
            }

            if(!string.IsNullOrEmpty(query.sortColumn) && IsValidProperty(query.sortColumn))
            {
                query.sortOrder = !string.IsNullOrEmpty(query.sortOrder) && query.sortOrder.ToUpper() == "ASC"
                    ? "ASC" : "DESC";
                source = source.OrderBy(string.Format("{0} {1}", query.sortColumn, query.sortOrder));
            }

            source = source.Skip(query.pageIndex * query.pageSize).Take(query.pageSize);
            return await source.ToListAsync();
        }

        /// <summary>
        /// Checks if the given property name exists to protect against SQL injection attacks
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="throwExceptionIfNotFound">if TRUE trhows exception if property doesn't exist</param>
        /// <returns>TRUE if property exists, FALSE otherwise</returns>
        /// <exception cref="NotSupportedException">Throws if property doesn't exist, and throwExceptionifNotFound set TRUE</exception>
        protected bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if(prop == null && throwExceptionIfNotFound)
            {
                throw new NotSupportedException($"Error: Property '{propertyName}' does not exist");
            }
            return prop != null;
        }

        
    }
}
