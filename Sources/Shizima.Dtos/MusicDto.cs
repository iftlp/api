using System.ComponentModel;

namespace Shizima.Dtos
{
    public class MusicDto
    {
        /// <summary>
        /// Gets or sets the music's id.
        /// </summary>
        [Description("The music's id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the music's title.
        /// </summary>
        [Description("The music's title")]
        public required string Title { get; set; }

        /// <summary>
        /// Get or sets the music's file path.
        /// </summary>
        [Description("The music's file path")]
        public required string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the music's duration.
        /// </summary>
        public long DurationInSec { get; set; }

        /// <summary>
        /// Gets or sets the music's artists.
        /// </summary>
        [Description("The music's artists")]
        public IEnumerable<ArtistDto>? Artists { get; set; }
        
        /// <summary>
        /// Gets or sets the music's album.
        /// </summary>
        [Description("The music's album")]
        public AlbumDto? Album { get; set; }
    }
}
