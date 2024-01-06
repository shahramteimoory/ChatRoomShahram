using Application.Interface.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Roles
{
    public interface IRoleService
    {
    }
    public class RoleService:BaseService, IRoleService
    {
        public RoleService(IDataBaseContext context) : base(context) { }
    }
}
