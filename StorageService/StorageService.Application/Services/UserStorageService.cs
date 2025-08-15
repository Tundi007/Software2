using StorageService.Application.Interfaces;
using StorageService.Contracts;
using StorageService.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Services
{
    public class UserStorageService : IUserStorageService
    {
        private readonly IUserStorageRepository _userStorageRepository;
        private readonly IStorageTypeRepository _storageTypeRepository;
        private readonly HttpClient _httpClient;
        private string checkUrl = "http://iam:8080/api/user/checkuserid/";
        public UserStorageService(IUserStorageRepository userStorageRepository, IStorageTypeRepository storageTypeRepository, HttpClient httpClient)
        {
            _userStorageRepository = userStorageRepository;
            _storageTypeRepository = storageTypeRepository;
            _httpClient = httpClient;
        }

        public async Task<UserStorage> Activate(int userStorageId)
        {
            var res = await _userStorageRepository.FindUserStorage(userStorageId);

            if (res == null)
            {
                return null;
            }
            else
            {
                res.IsActive = true;
                res = await _userStorageRepository.Update(res);
                return res;
            }
        }

        public async Task<UserStorageVM> Buy(UserStorageCreateVM userStorageCreateVM)
        {
            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            HttpResponseMessage response = await _httpClient.GetAsync(checkUrl + Convert.ToString(userStorageCreateVM.UserID));

            if (!response.IsSuccessStatusCode)
            {
                vm.UserID = -2;
                return vm;
            }


            StorageType storageType = await _storageTypeRepository.FindStorageType(userStorageCreateVM.StorageTypeID);
            UserStorage userStorage = new UserStorage() 
            {
                IsActive = false,
                EndDate = DateTime.Now.AddMonths(storageType.Month),
                StartDate = DateTime.Now,
                StorageTypeID = storageType.StorageTypeID,
                UserID = userStorageCreateVM.UserID,
                IsPublic = false
            };

            var res = await _userStorageRepository.Create(userStorage);
            if (res == null)
            {
                vm.UserID = -1;
            }
            else
            {
                vm.UserStorageId = res.UserStorageID;
                vm.UserID = res.UserID;
                vm.EndDate = res.EndDate;
                vm.StartDate = res.StartDate;
                vm.StorageType = new StorageTypeVM() 
                {
                    StorageTypeID = storageType.StorageTypeID,
                    Capacity = storageType.Capacity,
                    Month = storageType.Month,
                    Price = storageType.Price,
                    Title = storageType.Title
                };
                vm.IsPublic = false;
                vm.IsActive = false;
            }
            return vm;
        }


        public async Task<UserStorageVM> ExtendTime(ExtendTimeVM extendTimeVM)
        {
            UserStorageVM vm = new UserStorageVM();
            // create http content to send
            // send request using post
            HttpResponseMessage response = await _httpClient.GetAsync(checkUrl + Convert.ToString(extendTimeVM.UserID));

            if (!response.IsSuccessStatusCode)
            {
                vm.UserID = -2;
                return vm;
            }

            var userstorage = _userStorageRepository.GetUserStorages().FirstOrDefault(x => x.UserID == extendTimeVM.UserID);
            if (userstorage == null)
            {
                vm.UserID = -1;
                return vm;
            }
            userstorage.EndDate.AddMonths(extendTimeVM.Month);

            var res = await _userStorageRepository.Update(userstorage);

            if(res == null)
            {
                vm.UserID = -3;
                return vm;
            }
            vm.UserID = res.UserID;
            vm.EndDate = res.EndDate;
            vm.StartDate = res.StartDate;
            vm.StorageType = null;
            vm.IsActive = res.IsActive;
            vm.IsPublic = res.IsPublic;
            return vm;

        }

        public async Task<UserStorage> Find(int userStorageId)
        {
            var res =  await _userStorageRepository.FindUserStorage(userStorageId);
            if(res == null)
            {
                return null;
            }
            res.StorageType = await _storageTypeRepository.FindStorageType(res.StorageTypeID);
            if(res.IsActive == false)
            {
                return null;
            }
            return res;
        }
        public async Task<UserStorage> FindForActive(int userStorageId)
        {
            var res = await _userStorageRepository.FindUserStorage(userStorageId);
            res.StorageType = await _storageTypeRepository.FindStorageType(res.StorageTypeID);
            return res;
        }

        public List<UserStorage> GetUserStorages(int? userId)
        {
            if (userId == null)
            {
                var res = _userStorageRepository.GetUserStorages();
                foreach (var item in res)
                {
                    item.StorageType.UserStorages = null;
                }
                return res;
            }
            else
            {
                var res = _userStorageRepository.GetUserStorages().Where(x => x.UserID == userId).ToList();
                foreach (var item in res)
                {
                    item.StorageType.UserStorages = null;
                }
                return res;

            }
        }

        public async Task<bool> TogglePublic(int userStorageId)
        {
            var res = await _userStorageRepository.FindUserStorage(userStorageId);

            res.IsPublic = !res.IsPublic;

            var res2 = await _userStorageRepository.Update(res);

            return res2.IsPublic;
        }

    }
}
