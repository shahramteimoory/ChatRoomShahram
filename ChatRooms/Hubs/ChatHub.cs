using Application.Interface.FacadPatterns;
using ChatRooms.Utilities;
using Domain.Entites;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace ChatRooms.Hubs
{
    public class ChatHub:Hub, IChatHub
    {
        private readonly IChatsFacad _chatsFacad;
        private readonly IUserFacad _userFacad;
        public ChatHub(IChatsFacad chatsFacad, IUserFacad userFacad)
        {
            _chatsFacad = chatsFacad;
            _userFacad = userFacad;
        }

        public async Task joinGroup(string Token,long currentGroupId)
        {
            var group =await _chatsFacad.ChatGrpupsService.GetGroupBy(Token);
            if (!group.IsSuccess)
            {
              await  Clients.Caller.SendAsync("Error", "Group Not Found");
            }
            else
            {
                var chats =await _chatsFacad.ChatService.GetChats(group.Data.Id);
                if (!_userFacad.UserGroupsService.IsUserInGroup(Context.User.GetUserId(), Token).Result.IsSuccess)
                {
                    await _userFacad.UserGroupsService.InsertInGroup(Context.User.GetUserId(), group.Data.Id);
                    await Clients.Caller.SendAsync("NewGroup", group.Data.GroupTitle, group.Data.GroupToken, group.Data.ImageName);
                }
                if (currentGroupId!=0)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentGroupId.ToString());
                }
               
                await  Groups.AddToGroupAsync(Context.ConnectionId, group.Data.Id.ToString());
               await Clients.Group(group.Data.Id.ToString()).SendAsync("JoinGroup",group.Data, chats);
            }


        }

        public Task JoinPrivateGroup(long ReciveId, long currentGroupId)
        {
            throw new NotImplementedException();
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Welcome",Context.User.GetUserId());
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string text, long GroupId)
        {
            var group =await _chatsFacad.ChatGrpupsService.GetGroupBy(GroupId);
            if (!group.IsSuccess)
            {
                return;
            }
            var chat = new Chat()
            {
                ChatBody = text,
                GroupId = GroupId,
                createDate = DateTime.Now,
                UserId = Context.User.GetUserId(),

            };
           await _chatsFacad.ChatService.SendMessage(chat);
            var newchat = new ChatViewModel
            {
                ChatBody= chat.ChatBody,
                GroupId= GroupId,
                createDate=chat.createDate.ToString("HH:mm dd/MM"),
                UserId=chat.UserId,
                PersonName=Context.User.GetUserName(),
                GroupName= group.Data.GroupTitle
            };
            var UserIds =await _userFacad.UserGroupsService.GetUserIds(GroupId);
            await Clients.Users(UserIds.Data).SendAsync("ReciveNotification", newchat);
           await Clients.Group(GroupId.ToString()).SendAsync("ReciveMesage", newchat);
        }
    }
    public class ChatViewModel
    {
        public string ChatBody { get; set; }
        public string GroupName { get; set; }
        public long GroupId { get; set; }
        public string createDate { get; set; }
        public string PersonName { get;set; }
        public long UserId { get; set; }
    }
    public interface IChatHub
    {
        Task joinGroup(string Token, long currentGroupId);
        Task SendMessage(string text, long GroupId);
        Task JoinPrivateGroup(long ReciveId,long currentGroupId);
    }
}
