using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.common.Repositores;
using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService
{
    public class UserMangeService : IUserManageService
    {
        private readonly IUserRepository _userRepository;
        public UserMangeService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> CheckUser(int id)
        {
            return await _userRepository.Find(id);
        }

        public List<User> GetUsers()
        {
            return _userRepository.Get().ToList();
        }
    }
}
