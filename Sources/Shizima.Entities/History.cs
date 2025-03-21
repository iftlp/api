using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class History
    {
        /// <summary>
        /// Gets or sets the history user id.
        /// </summary>
        [Required]
        public long UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the history music id.
        /// </summary>
        [Required]
        public long MusicId { get; set; }
        
        /// <summary>
        /// Gets or sets the history recognition date.
        /// </summary>
        [Required]
        public DateTime RecognitionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the history user.
        /// </summary>
        public User User { get; set; }
        
        /// <summary>
        /// Gets or sets the history music.
        /// </summary>
        public Music? Music { get; set; }
    }    
}
