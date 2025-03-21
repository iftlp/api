using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user's token.
        /// </summary>
        [Required, MaxLength(100)]
        public required string Token { get; set; }
        
        /// <summary>
        /// Gets or sets the user's histories.
        /// </summary>
        public ICollection<History>? Histories { get; set; }
    } 
}
