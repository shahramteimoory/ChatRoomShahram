using Application.Interface.Context;
using Domain.Entites;
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
        Task SendMessage(Chat chat);
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
            }).ToListAsync();
        }

        public async Task SendMessage(Chat chat)
        {
            Insert(chat);
            await Save();
        }
    }
    public class Chats_Dto
    {
        public long UserId { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string PersonName { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }

    }
}
