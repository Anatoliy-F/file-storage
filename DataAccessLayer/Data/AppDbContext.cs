﻿using System;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
                    .HasForeignKey<AppFileData>(d => d.AppFileId)
                    .OnDelete(DeleteBehavior.Cascade);
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
