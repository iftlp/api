using Shizima.Dtos;
using Shizima.Entities;

namespace Shizima.Business.Mappers
{
    public static class AlbumMapper
    {
        /// <summary>
        /// Map an album entity to dto
        /// </summary>
        /// <param name="album">The album entity</param>
        /// <returns>The mapped album dto</returns>
        public static AlbumDto ToDto(this Album album)
        {
            return new AlbumDto
            {
                Id = album.Id,
                Title = album.Title,
                Cover = album.Cover,
                Musics = album.Musics.ToDtoNoAlbum(),
                Artist = album.Artist?.ToDto(),
            };
        }
        
        /// <summary>
        /// Map an album entity to dto without music
        /// </summary>
        /// <param name="album">The album entity</param>
        /// <returns>The mapped album dto</returns>
        public static AlbumDto ToDtoNoMusic(this Album album)
        {
            return new AlbumDto
            {
                Id = album.Id,
                Title = album.Title,
                Cover = album.Cover,
                Artist = album.Artist?.ToDto(),
            };
        }

        /// <summary>
        /// Map a list of album entities to list of dtos
        /// </summary>
        /// <param name="albums">List of album entities</param>
        /// <returns>The mapped list of album dtos</returns>
        public static IEnumerable<AlbumDto> ToDto(this IEnumerable<Album> albums)
        {
            return albums.Select(ToDto) ?? Enumerable.Empty<AlbumDto>();
        }

        /// <summary>
        /// Map a list of album entities to list of dtos without music
        /// </summary>
        /// <param name="albums">List of album entities</param>
        /// <returns>The mapped list of album dtos</returns>
        public static IEnumerable<AlbumDto> ToDtoNoMusic(this IEnumerable<Album> albums)
        {
            return albums.Select(ToDtoNoMusic) ?? Enumerable.Empty<AlbumDto>();
        }

        /// <summary>
        /// Map a album dto to entity
        /// </summary>
        /// <param name="albumDto">The album dto</param>
        /// <returns>The mapped album entity</returns>
        public static Album ToEntity(this AlbumDto albumDto)
        {
            return new Album
            {
                Id = albumDto.Id,
                Title = albumDto.Title,
                Cover = albumDto.Cover,
                ArtistId = albumDto.Artist?.Id ?? -1,
                Artist = albumDto.Artist?.ToEntity(),
            };
        }

        /// <summary>
        /// Map a list of album dto to entity
        /// </summary>
        /// <param name="albumDtos">The list of album dto</param>
        /// <returns>The mapped list of album entity</returns>
        public static IEnumerable<Album> ToEntity(this IEnumerable<AlbumDto> albumDtos)
        {
            return albumDtos.Select(ToEntity) ?? Enumerable.Empty<Album>();
        }

        /// <summary>
        /// Map a album dto to entity, with custom cover
        /// </summary>
        /// <param name="albumDto">The album dto</param>
        /// <returns>The mapped album entity</returns>
        public static Album ToEntity(this AlbumPostDto albumDto, string cover)
        {
            return new Album
            {
                Id = albumDto.Id,
                Title = albumDto.Title,
                Cover = cover,
                ArtistId = albumDto.ArtistId ?? -1,
            };
        }
    }
}
