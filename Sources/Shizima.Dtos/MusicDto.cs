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
        /// Gets or sets the music's album.
        /// </summary>
        [Description("The music's album")]
        public AlbumDto? Album { get; set; }
    }
}
