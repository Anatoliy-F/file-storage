using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Entities
{
    /// <summary>
    /// Base Class for apps entites. For usage in generic class/method
    /// </summary>
    public abstract class BaseEntity: IEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public Guid Id { get; set; }
    }
}
