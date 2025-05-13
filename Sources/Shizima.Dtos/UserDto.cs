using System.ComponentModel;

namespace Shizima.Dtos
{
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the user's id.
        /// </summary>
        [Description("The user's id")]
        public long Id { get; set; }
        
        /// <summary>
        /// Gets or sets the user's token.
        /// </summary>
        [Description("The user's token")]
        public string? Token { get; set; }
        
        /// <summary>
        /// Gets or sets the user's histories.
        /// </summary>
        [Description("The user's history")]
        public IEnumerable<HistoryDto>? Histories { get; set; }
    }   
}