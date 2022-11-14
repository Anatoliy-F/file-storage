using DataAccessLayer.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public abstract class BaseEntity: IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Used for concurrency checking
        /// </summary>
        [Timestamp]
        public byte[]? TimeStamp { get; set; }
    }
}
