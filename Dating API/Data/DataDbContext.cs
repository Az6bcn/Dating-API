using DatingAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Data
{
    public class DataDbContext: DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) {}

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>()
                .Property(pk => pk.LikeID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Like>()
                .HasKey(k => new { k.LikerUserID, k.LikeeUserID });


            modelBuilder.Entity<Like>()
                .HasOne(l => l.LikerUser)
                .WithMany(u => u.Likers)
                .HasForeignKey(fk => fk.LikerUserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
               .HasOne(l => l.LikeeUser)
               .WithMany(u => u.Likees)
               .HasForeignKey(fk => fk.LikeeUserID)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
