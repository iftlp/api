using Shizima.Dtos;
using Shizima.Entities;

namespace Shizima.Business.Mappers
{
    public static class MusicMapper
    {
        /// <summary>
        /// Map a music entity to dto
        /// </summary>
        /// <param name="music">The music entity</param>
        /// <returns>The mapped music dto</returns>
        public static MusicDto ToDto(this Music music)
        {
            return new MusicDto
            {
                Id = music.Id,
                Title = music.Title,
                Album = music.Album?.ToDto(),
            };
        }

        /// <summary>
        /// Map a list of music entities to list of dtos
        /// </summary>
        /// <param name="musics">List of music entities</param>
        /// <returns>The mapped list of music dtos</returns>
        public static IEnumerable<MusicDto> ToDto(this IEnumerable<Music>? musics)
        {
            return musics?.Select(ToDto) ?? Enumerable.Empty<MusicDto>();
        }

        /// <summary>
        /// Map a music dto to entity
        /// </summary>
        /// <param name="musicDto">The music dto</param>
        /// <returns>The mapped music entity</returns>
        public static Music ToEntity(this MusicDto musicDto)
        {
            return new Music
            {
                Id = musicDto.Id,
                Title = musicDto.Title,
                AlbumId = musicDto.Album?.Id ?? -1
            };
        }

        /// <summary>
        /// Map a list of music dto to entity
        /// </summary>
        /// <param name="musicDtos">The list of music dto</param>
        /// <returns>The mapped list of music entity</returns>
        public static IEnumerable<Music> ToEntity(this IEnumerable<MusicDto> musicDtos)
        {
            return musicDtos.Select(ToEntity) ?? Enumerable.Empty<Music>();
        }
    }
}
