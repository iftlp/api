using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class Artist : BaseEntity
    {
        /// <summary>
        /// Gets or sets the artist's name.
        /// </summary>
        [Required, MaxLength(100)]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the artist's musics.
        /// </summary>
        public ICollection<Music>? Musics { get; set; }
    }
}
