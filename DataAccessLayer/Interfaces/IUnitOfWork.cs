namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IAppFileDataRepository AppFileDataRepository { get; }
        
        IAppUserRepository AppUserRepository { get; }
        
        IShortLinkRepository ShortLinkRepository { get; }

        Task SaveAsync();
    }
}
