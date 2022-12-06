namespace DataAccessLayer.Interfaces
{
    /// <summary>
    /// Entitis with Guid primary key
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
