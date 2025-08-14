using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Interfaces
{
    public interface IStorageTypeRepository
    {
        List<StorageType> GetStorageTypes();
        Task<StorageType> FindStorageType(int id);
        Task<StorageType> Create(StorageType StorageType);
        Task<StorageType> Update(StorageType StorageType);
        Task Delete(int id);
    }
}
