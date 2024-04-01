using Application.Interface.FacadPatterns;
using Application.Service.Chats;
using Application.Service.Chats.ChatGroups;
using Application.Service.Users.FacadPatterns;
using Application.Service.Users.UserGroups;
using ChatRooms.Hubs;
using ChatRooms.Models;
using ChatRooms.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChatRooms.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IChatsFacad _chatsFacad;
        IHubContext<ChatHub> _ChatHub;
        IUserGroupsService _UserGroupsService;
        private readonly IUserFacad _userFacad;
        public HomeController(IChatsFacad chatsFacad, IHubContext<ChatHub> hubContext, IUserGroupsService userGroupsService, IUserFacad userFacad)
        {
            _ChatHub = hubContext;
            _chatsFacad = chatsFacad;
            _UserGroupsService = userGroupsService;
            _userFacad = userFacad;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _UserGroupsService.GetUserGroups(User.GetUserId());
            return View(model.Data);
        }
        [Authorize]
        [HttpPost]
        public async Task SendMassage([FromForm] InsertChatDto insertChatDto)
        {
            insertChatDto.UserId=User.GetUserId();
         var result=  await _chatsFacad.ChatService.SendMessage(insertChatDto);
            result.PersonName=User.GetUserName();
            var UserIds = await _userFacad.UserGroupsService.GetUserIds(insertChatDto.GroupId);
            await _ChatHub.Clients.Users(UserIds.Data).SendAsync("ReciveNotification", result);
            await _ChatHub.Clients.Group(insertChatDto.GroupId.ToString()).SendAsync("ReciveMesage", result);

        }
        [Authorize]
        [HttpPost]
        public async Task CreateGroup([FromForm]CreateGroup_Dto create)
        {
            try
            {
                create.UserId=User.GetUserId();
                var result = await _chatsFacad.ChatGrpupsService.InsertGroup(create);
                await _ChatHub.Clients.Users(User.GetUserId().ToString()).SendAsync("NewGroup", result.Data.GroupTitle, result.Data.GroupToken,result.Data.ImageName);
            }
            catch 
            {

                await _ChatHub.Clients.Users(User.GetUserId().ToString()).SendAsync("NewGroup","Error");
            }
        }
        public async Task<IActionResult> Search(string title)
        {
            return new ObjectResult( _chatsFacad.ChatGrpupsService.Search(title,User.GetUserId()).Result.Data);
        }
    }
}
