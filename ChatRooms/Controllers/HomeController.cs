using Application.Interface.FacadPatterns;
using Application.Service.Chats.ChatGroups;
using Application.Service.Users.UserGroups;
using ChatRooms.Hubs;
using ChatRooms.Models;
using ChatRooms.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace ChatRooms.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
      
        private readonly IChatsFacad _chatsFacad;
        IHubContext<ChatHub> _ChatHub;
        IUserGroupsService _UserGroupsService;
        public HomeController(IChatsFacad chatsFacad, IHubContext<ChatHub> hubContext, IUserGroupsService userGroupsService)
        {
            _ChatHub= hubContext;
            _chatsFacad = chatsFacad;
            _UserGroupsService = userGroupsService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await _UserGroupsService.GetUserGroups(User.GetUserId());
            return View(model.Data);
        }
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
        [Authorize]
        public async Task<IActionResult> Search(string title)
        {
            return new ObjectResult( _chatsFacad.ChatGrpupsService.Search(title,User.GetUserId()).Result.Data);
        }
    }
}
