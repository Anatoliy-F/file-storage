using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel;
using DataAccessLayer.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //To create migrations without DI
        /*public AppDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-OHE8HBB\SQLEXPRESS;Initial Catalog=FileStorage;trusted_connection=true;Encrypt=False");
        }*/
        //End of creating migrations without DI

        public DbSet<AppFileData> AppFilesData => Set<AppFileData>();
        public DbSet<AppUser> AppUsersData => Set<AppUser>();
        public DbSet<AppFile> AppFiles => Set<AppFile>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<AppFileData>(entity =>
            {
                entity.ToTable("FileData", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UntrustedName).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Note).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.Size).IsRequired();
                entity.Property(e => e.UploadDT).IsRequired();
                entity.Property(e => e.IsPublic).IsRequired();
                entity.Property(e => e.TimeStamp).IsRowVersion().IsConcurrencyToken();
                
                entity.HasOne(d => d.OwnerNav)
                    .WithMany(p => p.AppFiles)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_FileData_OwnerId");

                entity.HasOne(d => d.AppFileNav)
                    .WithOne(p => p.AppFileDataNav)
                    .HasForeignKey<AppFile>(d => d.AppFileDataId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ShortLink>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Link).IsRequired().HasMaxLength(16);
                entity.HasIndex(e => e.Link).IsUnique();
                entity.Property(e => e.TimeStamp);
            });

            builder.Entity<AppFile>(entity =>
            {
                entity.ToTable("Files", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.TimeStamp).IsRowVersion().IsConcurrencyToken();

                entity.HasIndex(e => e.AppFileDataId, "IX_Files_FileDataId");

                
            });

            //set one-to-one with ShortLink and AppFileData.
            builder.Entity<ShortLink>(entity => {
                entity.HasIndex(e => e.AppFileDataId, "IX_ShortLink_FileDataId").IsUnique();
            });

            builder.Entity<AppFileData>(entity => {
                entity.HasOne(d => d.ShortLinkNav)
                    .WithOne(p => p.AppFileDataNav)
                    .HasForeignKey<ShortLink>(d => d.AppFileDataId);
            });

            //set relationship between shared files and users who can view them
            builder.Entity<AppFileData>()
                .HasMany(p => p.FileViewers)
                .WithMany(u => u.ReadOnlyFiles)
                .UsingEntity<Dictionary<string, object>>(
                    "FileViewer",
                    j => j
                        .HasOne<AppUser>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_FileViewer_Users_UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<AppFileData>()
                        .WithMany()
                        .HasForeignKey("FileDataId")
                        .HasConstraintName("FK_FileViewer_FileData_FileDataId")
                        .OnDelete(DeleteBehavior.ClientCascade));
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TODO: log and handle
                throw new CustomConcurrencyException("A concurrency error happened.", ex);
            }
            catch (RetryLimitExceededException ex)
            {
                //TODO: log and handle
                throw new CustomRetryLimitExceededException("There is a problem with SQL Server.", ex);
            }
            catch (DbUpdateException ex)
            {
                //TODO: log and handle
                throw new CustomDbUpdateException("An error occurred updating the database", ex);
            }
            catch (Exception ex)
            {
                //TODO: log and handle
                throw new CustomException("An error occurred updating the database", ex);
            }
        }
    }
}
