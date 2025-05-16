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
        /// Gets or sets the album's musics.
        /// </summary>
        public ICollection<Music>? Musics { get; set; }

        /// <summary>
        /// Gets or sets the album's artist id.
        /// </summary>
        [Required]
        public required long ArtistId { get; set; }

        /// <summary>
        /// Gets or sets the album's artist.
        /// </summary>
        public Artist? Artist { get; set; }
    }
}
