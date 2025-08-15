using StorageService.Contracts;
using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Interfaces
{
    public interface IUserStorageService
    {
        Task<UserStorageVM> Buy(UserStorageCreateVM userStorageCreateVM);
        Task<UserStorageVM> ExtendTime(ExtendTimeVM extendTimeVM);
        List<UserStorage> GetUserStorages(int? userId);
        Task<UserStorage> Find(int userStorageId);
        Task<UserStorage> FindForActive(int userStorageId);
        public Task<bool> TogglePublic(int userStorageId);
        Task<UserStorage> Activate(int userStorageId);
    }
}
