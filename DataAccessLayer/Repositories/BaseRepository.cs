using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;


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

        public IQueryable<T> GetAll() => Table;

        public virtual async Task<T?> GetByIdAsync(Guid id) => await Table.FindAsync(id);

        public async Task AddAsync(T entity) => await Table.AddAsync(entity);

        public virtual void Delete(T entity) => Table.Remove(entity);

        public virtual void Update(T entity) => Table.Update(entity);

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
            if (disposing && _disposeContext)
            {
                Context.Dispose();
            }
            _isDisposed = true;
        }

        ~BaseRepository()
        {
            Dispose(false);
        }
    }
}
