namespace Shizima.Dtos
{
    public class MusicUploadRequestDto
    {
        public IFormFile File { get; set; }

        /// <summary>
        /// The music informations.
        /// </summary>
        public MusicPostDto Music { get; set; }
    }
}
