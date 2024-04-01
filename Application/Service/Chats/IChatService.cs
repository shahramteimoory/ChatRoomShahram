using Application.Interface.Context;
using Common;
using Domain.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Chats
{
    public interface IChatService
    {
        Task<ChatViewModel> SendMessage(InsertChatDto chat);
         Task<List<Chats_Dto>> GetChats(long GroupId);
    }
    public class ChatService:BaseService,IChatService
    {
       
        public ChatService(IDataBaseContext context):base(context) { }

        public async Task<List<Chats_Dto>> GetChats(long GroupId)
        {
            return await Table<Chat>().Include(u=>u.User).Include(u=>u.ChatGroup)
                .Where(g => g.GroupId == GroupId).Select(s=> new Chats_Dto
            {
                GroupId = s.GroupId,
                PersonName=s.User.UserName,
                Text=s.ChatBody,
                Date=s.createDate.ToString("HH:mm dd/MM"),
                GroupName=s.ChatGroup.GroupTitle,
                UserId =s.UserId,
                FileAttach=s.FileAttach,
            }).ToListAsync();
        }

        public async Task<ChatViewModel> SendMessage(InsertChatDto chat)
        {
            var group =await GetById<ChatGroup>(chat.GroupId);
            var chatModel = new Chat()
            {
                GroupId = chat.GroupId,
                UserId = chat.UserId,
            };

            if (chat.FileAttach != null)
            {
                var filename =await chat.FileAttach.SaveFile("wwwroot/files/");
                chatModel.ChatBody = chat.FileAttach.FileName;
                chatModel.FileAttach = filename;
                Insert(chatModel);
                await Save();
            }
            else
            {
                chatModel.ChatBody = chat.ChatBody;
                Insert(chatModel);
                await Save();
            }

            return new ChatViewModel()
            {
                createDate = chatModel.createDate.ToString("HH:mm dd/MM"),
                ChatBody = chatModel.ChatBody,
                GroupName = group.GroupTitle,
                GroupId = group.Id,
                UserId = chat.UserId,
                FileAttach= chatModel.FileAttach,
            };
        }
    }
    public class Chats_Dto
    {
        public string? FileAttach { get; set; }
        public long UserId { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string PersonName { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }

    }
    public class InsertChatDto
    {
        public string ChatBody { get; set; }
        public long GroupId { get; set; }
        public IFormFile? FileAttach { get; set; }
        public long UserId { get; set; }
    }
    public class ChatViewModel
    {
        public string? FileAttach { get; set; }
        public string ChatBody { get; set; }
        public string GroupName { get; set; }
        public long GroupId { get; set; }
        public string createDate { get; set; }
        public string PersonName { get; set; }
        public long UserId { get; set; }
    }
}
