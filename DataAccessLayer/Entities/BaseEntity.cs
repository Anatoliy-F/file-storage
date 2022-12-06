using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Entities
{
    public abstract class BaseEntity: IEntity
    {
        public Guid Id { get; set; }
    }
}
