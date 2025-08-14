using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.Interfaces
{
    public interface IUserManageService
    {
        Task<User> CheckUser(int id);
        List<User> GetUsers();
    }
}
