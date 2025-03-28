using Microsoft.EntityFrameworkCore;
using Shizima.Entities;

namespace Shizima.Providers
{
    /// <summary>
    /// DatabaseContext used for managing entities.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DatabaseContext() : base()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            InitializeArtistsMusicsRelation(modelBuilder);
            InitializeAlbumsMusicsRelation(modelBuilder);
            InitializeHistoryRelations(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void InitializeArtistsMusicsRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>().HasMany(a => a.Musics).WithMany(a => a.Artists);
        }

        private static void InitializeAlbumsMusicsRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>().HasMany(a => a.Musics).WithOne(m => m.Album).HasForeignKey(m => m.AlbumId);
        }
        
        private static void InitializeHistoryRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>().HasKey(h => new {h.UserId, h.MusicId});
            modelBuilder.Entity<History>().HasOne(h => h.User)
                                            .WithMany(u => u.Histories)
                                            .HasForeignKey(h => h.UserId);
            modelBuilder.Entity<History>().HasOne(h => h.Music)
                                            .WithMany(m => m.Histories)
                                            .HasForeignKey(h => h.MusicId);
        }
    }
}