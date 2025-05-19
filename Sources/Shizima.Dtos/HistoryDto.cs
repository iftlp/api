using System.ComponentModel;

namespace Shizima.Dtos
{
    public class HistoryDto
    {
        /// <summary>
        /// Gets or sets the history's id.
        /// </summary>
        [Description("The history's id")]
        public long Id { get; set; }
        
        /// <summary>
        /// Gets or sets the history's recognition date.
        /// </summary>
        [Description("The history's recognition date")]
        public required DateTime RecognitionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the history's music.
        /// </summary>
        [Description("The history's music")]
        public required MusicDto Music { get; set; }
    }   
}