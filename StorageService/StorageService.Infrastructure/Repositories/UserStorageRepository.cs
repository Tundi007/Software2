using Microsoft.EntityFrameworkCore;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using StorageService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Infrastructure.Repositories
{
    public class UserStorageRepository : IUserStorageRepository
    {
        private readonly SQLServerContext _context;

        public UserStorageRepository(SQLServerContext context)
        {
            _context = context;
        }

        public async Task<UserStorage> Create(UserStorage UserStorage)
        {
            try
            {
                await _context.UserStorages.AddAsync(UserStorage);
                await _context.SaveChangesAsync();
                return UserStorage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                UserStorage UserStorage = await _context.UserStorages.FindAsync(id);
                _context.UserStorages.Remove(UserStorage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<UserStorage> FindUserStorage(int id)
        {
            return await _context.UserStorages.FindAsync(id);
        }

        public List<UserStorage> GetUserStorages()
        {
            return _context.UserStorages.Include(x => x.StorageType).ToList();
        }

        public async Task<UserStorage> Update(UserStorage UserStorage)
        {
            try
            {
                _context.UserStorages.Update(UserStorage);
                await _context.SaveChangesAsync();
                return UserStorage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
