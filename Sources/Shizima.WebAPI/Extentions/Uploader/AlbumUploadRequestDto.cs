using Shizima.Dtos;

namespace Shizima.WebAPI.Extentions.Uploader
{
    public class AlbumUploadRequestDto
    {
        /// <summary>
        /// The cover file to upload.
        /// </summary>
        public IFormFile Cover { get; set; }

        /// <summary>
        /// The album informations.
        /// </summary>
        public AlbumPostDto Album { get; set; }
    }
}
