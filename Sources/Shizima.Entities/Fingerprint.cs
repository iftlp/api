namespace Shizima.Entities
{
    /// <summary>
    /// Represent an audio fingerprint
    /// </summary>
    public class DatabaseFingerprint : BaseEntity
    {
        /// <summary>
        /// Hash value of the fingerprint
        /// </summary>
        public uint Hash { get; set; }

        /// <summary>
        /// Time offset from the audio begining
        /// </summary>
        public int TimeOffset { get; set; }

        /// <summary>
        /// The corresponding music id
        /// </summary>
        public long MusicId { get; set; }
    }
}
