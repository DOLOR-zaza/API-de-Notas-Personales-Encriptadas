using Microsoft.EntityFrameworkCore;
using API_BACKEND1.Models;

namespace API_BACKEND1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<SharedNote> SharedNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.Note)
                .WithMany()
                .HasForeignKey(sn => sn.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.SharedByUser)
                .WithMany()
                .HasForeignKey(sn => sn.SharedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.SharedWithUser)
                .WithMany()
                .HasForeignKey(sn => sn.SharedWithUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
