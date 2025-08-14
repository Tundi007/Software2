using StorageService.Contracts;
using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Interfaces
{
    public interface IStorageTypeService
    {
        List<StorageTypeVM> Get();
        Task<StorageTypeVM> Find(int id);
        Task<StorageTypeVM> Create(StorageTypeVMCreate storageTypeVM);
        Task<StorageTypeVM> Update(StorageTypeVM storageTypeVM);
        Task Delete(int id);
    }
}
