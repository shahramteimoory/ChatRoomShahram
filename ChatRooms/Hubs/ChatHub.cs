using Application.Interface.FacadPatterns;
using Application.Service.Chats;
using ChatRooms.Utilities;
using Domain.Entites;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Common;
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
                var groupDto = FixGroupModel(group.Data);
                var chats =await _chatsFacad.ChatService.GetChats(group.Data.Id);
                if (!_userFacad.UserGroupsService.IsUserInGroup(Context.User.GetUserId(), Token).Result.IsSuccess)
                {
                    await _userFacad.UserGroupsService.InsertInGroup(Context.User.GetUserId(), group.Data.Id);
                    await Clients.Caller.SendAsync("NewGroup", groupDto.GroupTitle, groupDto.GroupToken, groupDto.ImageName);
                }
                if (currentGroupId!=0)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentGroupId.ToString());
                }
               
                await  Groups.AddToGroupAsync(Context.ConnectionId, group.Data.Id.ToString());
               await Clients.Caller.SendAsync("JoinGroup",groupDto, chats);
            }


        }

        public async Task JoinPrivateGroup(long ReciveId, long currentGroupId)
        {
            if (currentGroupId != 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentGroupId.ToString());
            }
            var group =await _chatsFacad.ChatGrpupsService.InsertPrivateGroup(Context.User.GetUserId(), ReciveId);
            var groupDto = FixGroupModel(group.Data);
            if (!_userFacad.UserGroupsService.IsUserInGroup(Context.User.GetUserId(), group.Data.GroupToken).Result.IsSuccess)
            {
                await _userFacad.UserGroupsService.InsertInGroup(new List<long>() { group.Data.OwnerId, group.Data.ReciverId.Value }, group.Data.Id);


                await Clients.Caller.SendAsync("NewGroup", groupDto.GroupTitle, groupDto.GroupToken, groupDto.ImageName);
                await Clients.User(groupDto.ReciverId.ToString()).SendAsync("NewGroup", Context.User.GetUserName(), groupDto.GroupToken, groupDto.ImageName);
            }
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Data.Id.ToString());

            var chats = await _chatsFacad.ChatService.GetChats(group.Data.Id);


            await Clients.Caller.SendAsync("JoinGroup", groupDto, chats);
        }
        private ChatGroup FixGroupModel(ChatGroup chatGroup)
        {
            if (chatGroup.IsPrivete)
            {
                if (chatGroup.OwnerId==Context.User.GetUserId())
                {
                    return new ChatGroup()
                    {
                        Id = chatGroup.Id,
                        GroupToken = chatGroup.GroupToken,
                        createDate = chatGroup.createDate,
                        GroupTitle = chatGroup.Reciver.UserName,
                        ImageName = chatGroup.Reciver.Avatar,
                        IsPrivete = false,
                        OwnerId = chatGroup.OwnerId,
                        ReciverId = chatGroup.ReciverId,
                    };

                }
                return new ChatGroup()
                {
                    Id = chatGroup.Id,
                    GroupToken = chatGroup.GroupToken,
                    createDate = chatGroup.createDate,
                    GroupTitle = chatGroup.User.UserName,
                    ImageName = chatGroup.User.Avatar,
                    IsPrivete = false,
                    OwnerId = chatGroup.OwnerId,
                    ReciverId = chatGroup.ReciverId,
                };
            }
            return new ChatGroup()
            {
                Id = chatGroup.Id,
                GroupToken=chatGroup.GroupToken,
                createDate=chatGroup.createDate,
                GroupTitle=chatGroup.GroupTitle,
                ImageName=chatGroup.ImageName,
                IsPrivete=false,
                OwnerId=chatGroup.OwnerId,
                ReciverId=chatGroup.ReciverId,
            };
        }
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Welcome",Context.User.GetUserId());
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string text, long GroupId)
        {
           // var group =await _chatsFacad.ChatGrpupsService.GetGroupBy(GroupId);
           // if (!group.IsSuccess)
           // {
           //     return;
           // }
           // var chat = new Chat()
           // {
           //     ChatBody = text,
           //     GroupId = GroupId,
           //     createDate = DateTime.Now,
           //     UserId = Context.User.GetUserId(),

           // };
           //await _chatsFacad.ChatService.SendMessage(chat);
           // var newchat = new ChatViewModel
           // {
           //     ChatBody= chat.ChatBody,
           //     GroupId= GroupId,
           //     createDate=chat.createDate.ToString("HH:mm dd/MM"),
           //     UserId=chat.UserId,
           //     PersonName=Context.User.GetUserName(),
           //     GroupName= group.Data.GroupTitle
           // };

        }
    }

    public interface IChatHub
    {
        Task joinGroup(string Token, long currentGroupId);
        Task SendMessage(string text, long GroupId);
        Task JoinPrivateGroup(long ReciveId,long currentGroupId);
    }
}
