using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Interfaces
{
    public interface IUserStorageRepository
    {
        List<UserStorage> GetUserStorages();
        Task<UserStorage> FindUserStorage(int id);
        Task<UserStorage> Create(UserStorage UserStorage);
        Task<UserStorage> Update(UserStorage UserStorage);
        Task Delete(int id);
    }
}
