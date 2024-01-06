using Application.Service.Users;
using Application.Service.Users.UserGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.FacadPatterns
{
    public interface IUserFacad
    {
        IUserService UserService { get; }
        IUserGroupsService UserGroupsService { get; }
    }
}
