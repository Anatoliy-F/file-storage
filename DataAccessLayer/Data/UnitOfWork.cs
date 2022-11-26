﻿using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IAppUserRepository? _userRepository;
        private IAppFileDataRepository? _fileDataRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IAppFileDataRepository AppFileDataRepository => _fileDataRepository ??= new AppFileDataRepository(_context);

        public IAppUserRepository AppUserRepository => _userRepository ??= new AppUserRepository(_context);


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed && disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}