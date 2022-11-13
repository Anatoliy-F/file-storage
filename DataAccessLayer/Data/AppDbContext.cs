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
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppFileData> AppFilesData => Set<AppFileData>();
        public DbSet<AppUser> AppUsersData => Set<AppUser>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //TODO: setup entities with Fluent API

            //set ono-to-one with File and FileData. File choosed as principal (because app retrive AppFileData at first)
            builder.Entity<AppFileData>(entity =>
            {
                entity.HasIndex(e => e.AppFileId, "IX_FileData_FileId").IsUnique();
            });

            builder.Entity<AppFile>(entity =>
            {
                entity.HasOne(d => d.AppFileDataNav)
                    .WithOne(p => p.AppFileNav)
                    .HasPrincipalKey<AppFileData>(d => d.AppFileId)
                    .OnDelete(DeleteBehavior.Cascade);
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
