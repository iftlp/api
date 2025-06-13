namespace Shizima.Entities
{
    /// <summary>
    /// Represent an audio fingerprint
    /// </summary>
    public class JsonFingerprint : BaseEntity
    {
        /// <summary>
        /// Hash value of the fingerprint
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// The corresponding music id
        /// </summary>
        public long MusicId { get; set; }
    }
}
