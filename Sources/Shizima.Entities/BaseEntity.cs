using System.ComponentModel.DataAnnotations.Schema;

namespace Shizima.Entities
{
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity's id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}
