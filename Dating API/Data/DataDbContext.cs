using DatingAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DatingAPI.Data
{
    public class DataDbContext: DbContext
    {

        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) {}

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        //}

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


        //public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[]
        //                                                        {
        //                                                            new ConsoleLoggerProvider((category, level)
        //                                                                => category == DbLoggerCategory.Database.Command.Name
        //                                                                   && level == LogLevel.Information, true)
        //                                                        });
    }
}
