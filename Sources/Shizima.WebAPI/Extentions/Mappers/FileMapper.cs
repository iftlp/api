namespace Shizima.WebAPI.Extentions.Mappers
{
    public static class FileMapper
    {
        public static async Task<byte[]> ToBytes(this IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
