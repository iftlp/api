using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Shizima.Entities
{
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user's token.
        /// </summary>
        public string? Token { get; set; }
        
        /// <summary>
        /// Gets or sets the user's histories.
        /// </summary>
        public ICollection<History>? Histories { get; set; }
    } 
}
