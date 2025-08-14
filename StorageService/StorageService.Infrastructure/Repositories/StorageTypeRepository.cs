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
    public class StorageTypeRepository : IStorageTypeRepository
    {
        private readonly SQLServerContext _context;

        public StorageTypeRepository(SQLServerContext context)
        {
            _context = context;
        }

        public async Task<StorageType> Create(StorageType StorageType)
        {
            try
            {
                await _context.StorageTypes.AddAsync(StorageType);
                await _context.SaveChangesAsync();
                return StorageType;
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
                StorageType storageType = await _context.StorageTypes.FindAsync(id);
                _context.StorageTypes.Remove(storageType);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<StorageType> FindStorageType(int id)
        {
            return await _context.StorageTypes.FindAsync(id);
        }

        public List<StorageType> GetStorageTypes()
        {
            return _context.StorageTypes.ToList();
        }

        public async Task<StorageType> Update(StorageType StorageType)
        {
            try
            {
                _context.StorageTypes.Update(StorageType);
                await _context.SaveChangesAsync();
                return StorageType;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
