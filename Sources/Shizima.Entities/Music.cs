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
        /// Gets or sets the music's file path.
        /// </summary>
        [Required, MaxLength(500)]
        public required string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the music's duration.
        /// </summary>
        public long DurationInSec { get; set; }

        /// <summary>
        /// Gets or sets the music's album id.
        /// </summary>
        [Required]
        public required long AlbumId { get; set; }

        /// <summary>
        /// Gets or sets the music's album.
        /// </summary>
        public Album? Album { get; set; }
        
        /// <summary>
        /// Gets or sets the music's artists.
        /// </summary>
        public ICollection<Artist>? Artists { get; set; }
        
        /// <summary>
        /// Gets or sets the music's histories.
        /// </summary>
        public ICollection<History>? Histories { get; set; }

        /// <summary>
        /// Gets or sets the music's fingerprints
        /// </summary> 
        public ICollection<DatabaseFingerprint>? Fingerprints { get; set; }

        /// <summary>
        /// Gets or sets the music's fingerprints
        /// </summary> 
        public ICollection<JsonFingerprint>? JsonFingerprints { get; set; }
    }
}
