using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class Artist : BaseEntity
    {
        /// <summary>
        /// Gets or sets the artist's name.
        /// </summary>
        [Required, MaxLength(64)]
        public string Name { get; set; }
    }
}
