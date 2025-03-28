using System.ComponentModel.DataAnnotations;

namespace Shizima.Entities
{
    public class Administrator : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user's name
        /// </summary>
        [MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password
        /// </summary>
        public string? Password { get; set; }
    }  
}
