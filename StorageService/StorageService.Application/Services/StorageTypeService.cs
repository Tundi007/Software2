using StorageService.Application.Interfaces;
using StorageService.Contracts;
using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Services
{
    public class StorageTypeService : IStorageTypeService
    {
        private readonly IStorageTypeRepository _storageTypeRepository;
        public StorageTypeService(IStorageTypeRepository storageTypeRepository)
        {
            _storageTypeRepository = storageTypeRepository;
        }
        public async Task<StorageTypeVM> Create(StorageTypeVMCreate storageTypeVM)
        {
            StorageType storageType = new StorageType();
            storageType.Capacity = storageTypeVM.Capacity;
            storageType.Month = storageTypeVM.Month;
            storageType.Price = storageTypeVM.Price;
            storageType.Title = storageTypeVM.Title;
            var res = await _storageTypeRepository.Create(storageType);
            StorageTypeVM typeVM = new StorageTypeVM();
            if (res != null)
            {
                typeVM.Price = res.Price;
                typeVM.Capacity = res.Capacity;
                typeVM.Month = res.Month;
                typeVM.Title = res.Title;
                typeVM.StorageTypeID = res.StorageTypeID;
            }
            else
            {
                typeVM.StorageTypeID = -1;
            }

            return typeVM;
        }

        public async Task Delete(int id)
        {
            await _storageTypeRepository.Delete(id);
        }

        public async Task<StorageTypeVM> Find(int id)
        {
            var res = await _storageTypeRepository.FindStorageType(id);

            if(res == null)
            {
                return null;
            }

            StorageTypeVM typeVM = new StorageTypeVM();
            typeVM.Price = res.Price;
            typeVM.Capacity = res.Capacity;
            typeVM.Month = res.Month;
            typeVM.Title = res.Title;
            typeVM.StorageTypeID = res.StorageTypeID;


            return typeVM;
        }

        public List<StorageTypeVM> Get()
        {
            var res = _storageTypeRepository.GetStorageTypes();
            List<StorageTypeVM> storageTypeVMs = new List<StorageTypeVM>();

            foreach (var item in res)
            {
                StorageTypeVM typeVM = new StorageTypeVM();
                typeVM.Price = item.Price;
                typeVM.Capacity = item.Capacity;
                typeVM.Month = item.Month;
                typeVM.Title = item.Title;
                typeVM.StorageTypeID = item.StorageTypeID;
                storageTypeVMs.Add(typeVM);
            }

            return storageTypeVMs;
        }

        public async Task<StorageTypeVM> Update(StorageTypeVM storageTypeVM)
        {
            StorageType storageType = new StorageType();
            storageType.Capacity = storageTypeVM.Capacity;
            storageType.Month = storageTypeVM.Month;
            storageType.Price = storageTypeVM.Price;
            storageType.Title = storageTypeVM.Title;
            storageType.StorageTypeID = storageTypeVM.StorageTypeID;


            var res = await _storageTypeRepository.Update(storageType);
            if (res == null)
            {
                storageTypeVM.StorageTypeID = -1;
            }

            return storageTypeVM;
        }
    }
}
