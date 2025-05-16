using System.ComponentModel;

namespace Shizima.Dtos
{
    public class AlbumDto
    {
        /// <summary>
        /// Gets or sets the album's id.
        /// </summary>
        [Description("The album's id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the album's title.
        /// </summary>
        [Description("The album's title")]
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the album's cover.
        /// </summary>
        [Description("The album's cover")]
        public string? Cover { get; set; }

        /// <summary>
        /// Gets or sets the album's musics.
        /// </summary>
        [Description("The album's musics")]
        public IEnumerable<MusicDto>? Musics { get; set; }

        /// <summary>
        /// Gets or sets the album's artist.
        /// </summary>
        public ArtistDto? Artist { get; set; }
    }
}
