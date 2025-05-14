using Shizima.Dtos;
using Shizima.Entities;

namespace Shizima.Business.Mappers
{
    public static class ArtistMapper
    {
        /// <summary>
        /// Map an artist entity to dto
        /// </summary>
        /// <param name="artist">The artist entity</param>
        /// <returns>The mapped artist dto</returns>
        public static ArtistDto ToDto(this Artist artist)
        {
            return new ArtistDto
            {
                Id = artist.Id,
                Name = artist.Name,
            };
        }

        /// <summary>
        /// Map a list of artist entities to list of dtos
        /// </summary>
        /// <param name="artists">List of artist entities</param>
        /// <returns>The mapped list of artist dtos</returns>
        public static IEnumerable<ArtistDto> ToDto(this IEnumerable<Artist>? artists)
        {
            return artists?.Select(ToDto) ?? Enumerable.Empty<ArtistDto>();
        }

        /// <summary>
        /// Map an artist dto to entity
        /// </summary>
        /// <param name="artistDto">The artist dto</param>
        /// <returns>The mapped artist entity</returns>
        public static Artist ToEntity(this ArtistDto artistDto)
        {
            return new Artist
            {
                Id = artistDto.Id,
                Name = artistDto.Name
            };
        }

        /// <summary>
        /// Map a list of artists dto to entity
        /// </summary>
        /// <param name="artistDtos">The list of artist dto</param>
        /// <returns>The mapped list of artist entity</returns>
        public static IEnumerable<Artist> ToEntity(this IEnumerable<ArtistDto> artistDtos)
        {
            return artistDtos.Select(ToEntity) ?? Enumerable.Empty<Artist>();
        }
    }   
}