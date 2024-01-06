using Application.Interface.Context;
using Application.Interface.FacadPatterns;
using Application.Service.Chats.ChatGroups;
using Application.Service.Roles;
using Application.Service.Users.UserGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Chats.FacadPatterns
{
    public class ChatsFacad : IChatsFacad
    {
        private readonly IDataBaseContext _context;
        private readonly IUserGroupsService _userGroupsService;
        public ChatsFacad(IDataBaseContext context, IUserGroupsService userGroupsService)
        {
            _context=context;
            _userGroupsService=userGroupsService;
        }
        private IChatService? _ChatService;
        public IChatService ChatService
        {
            get
            {
                return _ChatService = _ChatService ?? new ChatService(_context);
            }
        }
        private IChatGrpupsService? _ChatGrpupsService;
        public IChatGrpupsService ChatGrpupsService
        {
            get
            {
                return _ChatGrpupsService= _ChatGrpupsService ?? new ChatGrpupsService(_context, _userGroupsService);
            }
        }
    }
}
