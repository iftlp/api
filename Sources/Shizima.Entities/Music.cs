using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class Music : BaseEntity
    {
        /// <summary>
        /// Gets or sets the music's title.
        /// </summary>
        [Required, MaxLength(100)]
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the music's cover.
        /// </summary>
        public string? Cover { get; set; }

        /// <summary>
        /// Gets or sets the music's album id.
        /// </summary>
        public long AlbumId { get; set; }

        /// <summary>
        /// Gets or sets the music's album.
        /// </summary>
        public Album? Album { get; set; }
    }
}
