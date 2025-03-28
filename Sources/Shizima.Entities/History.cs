using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class History
    {
        /// <summary>
        /// Gets or sets the history user id.
        /// </summary>
        [Required]
        public required long UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the history music id.
        /// </summary>
        [Required]
        public required long MusicId { get; set; }
        
        /// <summary>
        /// Gets or sets the history recognition date.
        /// </summary>
        [Required]
        public required DateTime RecognitionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the history user.
        /// </summary>
        public required User User { get; set; }
        
        /// <summary>
        /// Gets or sets the history music.
        /// </summary>
        public required Music Music { get; set; }
    }    
}
