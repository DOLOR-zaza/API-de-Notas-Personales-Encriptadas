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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE: Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // SharedNote: Relación con Nota
            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.Note)
                .WithMany()
                .HasForeignKey(sn => sn.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            // SharedNote: Relación con Usuario que comparte (Restrict para evitar ciclos)
            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.SharedByUser)
                .WithMany()
                .HasForeignKey(sn => sn.SharedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SharedNote: Relación con Usuario destino (Restrict para evitar ciclos)
            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.SharedWithUser)
                .WithMany()
                .HasForeignKey(sn => sn.SharedWithUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar compartir la misma nota 2 veces al mismo usuario (Índice Compuesto)
            modelBuilder.Entity<SharedNote>()
                .HasIndex(sn => new { sn.NoteId, sn.SharedWithUserId })
                .IsUnique();

            // Permission (catálogo)
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<SharedNote>()
                .HasOne(sn => sn.Permission)
                .WithMany(p => p.SharedNotes)
                .HasForeignKey(sn => sn.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed de permisos
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Code = "READ", Description = "Solo lectura" },
                new Permission { Id = 2, Code = "WRITE", Description = "Lectura y escritura" }
            );

            // AuditLog: Relaciones
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Note)
                .WithMany()
                .HasForeignKey(a => a.NoteId)
                .OnDelete(DeleteBehavior.SetNull); // Nota: NoteId debe ser nullable (int?) en tu modelo
        }
    }
}