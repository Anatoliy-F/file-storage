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
        public Task<ServiceResponse<FileDataModel>> GenerateForFileByIdAsync(Guid fileId);

        public Task<ServiceResponse<FileDataModel>> GetFileByShortLinkAsync(string link);

        public Task<ServiceResponse<bool>> DeleteLinkAsync(string link);

        public Task<ServiceResponse<ShortFileDataModel>> GetShortFileDataAsync(string link);
    }
}
