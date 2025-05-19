using Shizima.Dtos;
using Shizima.Entities;

namespace Shizima.Business.Mappers
{
    public static class HistoryMapper
    {
        /// <summary>
        /// Map an history entity to dto
        /// </summary>
        /// <param name="history">The history entity</param>
        /// <returns>The mapped history dto</returns>
        public static HistoryDto ToDto(this History history)
        {
            return new HistoryDto
            {
                Id = history.Id,
                RecognitionDate = history.RecognitionDate,
                Music = history.Music.ToDto(),
            };
        }

        /// <summary>
        /// Map a list of history entities to list of dtos
        /// </summary>
        /// <param name="historys">List of history entities</param>
        /// <returns>The mapped list of history dtos</returns>
        public static IEnumerable<HistoryDto> ToDto(this IEnumerable<History>? historys)
        {
            return historys?.Select(ToDto) ?? Enumerable.Empty<HistoryDto>();
        }
    } 
}