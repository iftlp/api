using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class Album : BaseEntity
    {
        /// <summary>
        /// Gets or sets the album's title.
        /// </summary>
        [Required, MaxLength(100)]
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the album's cover.
        /// </summary>
        public string? Cover { get; set; }

        /// <summary>
        /// Gets or sets the album's artists.
        /// </summary>
        public ICollection<Artist>? Artists { get; set; }

        /// <summary>
        /// Gets or sets the album's musics.
        /// </summary>
        public ICollection<Music>? Musics { get; set; }
    }
}
