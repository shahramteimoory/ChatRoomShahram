using Application.Interface.Context;
using Application.Interface.FacadPatterns;
using Application.Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Roles.FacadPatterns
{
    public class RoleFacad : IRoleFacad
    {
        private readonly IDataBaseContext _context;
        public RoleFacad(IDataBaseContext context)
        {
            _context=context;
        }
        private IRoleService? _RoleService;
        public IRoleService RoleService
        {
            get
            {
                return _RoleService = _RoleService ?? new RoleService(_context);
            }
        }
    }
}
