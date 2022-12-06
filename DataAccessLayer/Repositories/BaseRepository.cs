using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// Provide basic fuctionality
    /// implement IDisposable
    /// </summary>
    /// <typeparam name="T">Entitis with Guid primary key</typeparam>
    public abstract class BaseRepository<T> : IDisposable, IRepository<T> where T : class, IEntity, new()
    {
        private readonly bool _disposeContext;
       
        public DbSet<T> Table { get; }
        public AppDbContext Context { get; }

        /// <summary>
        /// Perform basic object setup
        /// </summary>
        /// <param name="context">AppDbContext instanse</param>
        protected BaseRepository(AppDbContext context)
        {
            Context = context;
            Table = Context.Set<T>();
            _disposeContext = false;
        }

        /// <summary>
        /// Contructor to interact with cli ef tools
        /// </summary>
        /// <param name="options">DbContextOptions<AppDbContext> options</param>
        protected BaseRepository(DbContextOptions<AppDbContext> options) : this(new AppDbContext(options))
        {
            _disposeContext = true;
        }

        // <inheritdoc />
        public IQueryable<T> GetAll() => Table;

        // <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(Guid id) => await Table.FindAsync(id);

        // <inheritdoc />
        public async Task AddAsync(T entity) => await Table.AddAsync(entity);

        // <inheritdoc />
        public virtual void Delete(T entity) => Table.Remove(entity);

        // <inheritdoc />
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
