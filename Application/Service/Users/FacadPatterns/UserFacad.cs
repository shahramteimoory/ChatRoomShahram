using Application.Interface.Context;
using Application.Interface.FacadPatterns;
using Application.Service.Users.UserGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users.FacadPatterns
{
    public class UserFacad : IUserFacad
    {
        private readonly IDataBaseContext _context;
        public UserFacad(IDataBaseContext context)
        {
            _context=context;
        }
        private IUserService? _UserService;
        public IUserService UserService
        {
            get
            {
              return  _UserService= _UserService?? new UserService(_context);
            }
        }
        private IUserGroupsService _UserGroupsService;
        public IUserGroupsService UserGroupsService
        {
            get
            {
                return _UserGroupsService= _UserGroupsService??new UserGroupsService(_context);
            }
        }
    }
}
