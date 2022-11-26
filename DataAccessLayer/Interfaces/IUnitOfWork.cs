namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IAppFileDataRepository AppFileDataRepository { get; }
        
        IAppUserRepository AppUserRepository { get; }
        
        Task SaveAsync();
    }
}
