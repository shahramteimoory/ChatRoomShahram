using Application.Service.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.FacadPatterns
{
    public interface IRoleFacad
    {
        IRoleService RoleService { get; }
    }
}
