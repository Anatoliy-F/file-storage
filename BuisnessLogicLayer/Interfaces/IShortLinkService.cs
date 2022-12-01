using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Interfaces
{
    public interface IShortLinkService
    {
        public Task<(bool IsSuccess, FileDataModel? Data, string? ErrorMessage)> GenerateForFileById(Guid fileId);

        public Task<(bool IsSuccess, AppFileData? Data, string? ErrorMessage)> GetFileByShortLinkAsync(string link);

        public Task<(bool IsSuccess, FileDataModel? Data, string? ErrorMessage)> DeleteLinkAsync(string link);
    }
}
