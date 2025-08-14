using IAM.Contracts.User;
using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.Interfaces
{
    public interface IUserUpdateService
    {
        Task<UserUpdateVM> handle(UserUpdateVM userUpdate);
        Task<User> handle(AvatarVM avatar);
    }
}
