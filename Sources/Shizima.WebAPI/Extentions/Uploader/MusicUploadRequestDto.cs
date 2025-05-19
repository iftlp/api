namespace Shizima.Dtos
{
    public class MusicUploadRequestDto
    {
        /// <summary>
        /// The file to upload.
        /// </summary>
        public IFormFile AudioFile { get; set; }

        /// <summary>
        /// The music informations.
        /// </summary>
        public MusicPostDto Music { get; set; }
    }
}
