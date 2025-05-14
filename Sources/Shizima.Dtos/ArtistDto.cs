using System.ComponentModel;

namespace Shizima.Dtos
{
    public class ArtistDto
    {
        /// <summary>
        /// Gets or sets the artist's id.
        /// </summary>
        [Description("The artist's id")]
        public long Id { get; set; }
        
        /// <summary>
        /// Gets or sets the artist's name.
        /// </summary>
        [Description("The artist's name")]
        public required string Name { get; set; }
    } 
}