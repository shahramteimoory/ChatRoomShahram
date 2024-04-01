using Application.Interface.Context;
using Application.Service.Users.UserGroups;
using Common;
using Common.Dto;
using Domain.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.FacadPatterns;

namespace Application.Service.Chats.ChatGroups
{
    public interface IChatGrpupsService
    {
        Task<ResultDto<List<Search_ResultDto>>> Search(string Title, long UserId);
        Task<ResultDto<List<ChatGroup>>> GetUserGroups(long UserId);
        Task<ResultDto<ChatGroup>> InsertGroup(CreateGroup_Dto req);
        Task<ResultDto<ChatGroup>> GetGroupBy(long id);
        Task<ResultDto<ChatGroup>> GetGroupBy(string token);
        Task<ResultDto<ChatGroup>> InsertPrivateGroup(long UserId,long ReciverId);
    }
    public class ChatGrpupsService : BaseService, IChatGrpupsService
    {
        private readonly IUserGroupsService _userGroupsService;
        public ChatGrpupsService(IDataBaseContext context, IUserGroupsService userGroupsService) : base(context)
        {
            _userGroupsService = userGroupsService;
        }

        public async Task<ResultDto<ChatGroup>> GetGroupBy(long id)
        {
            var result = await Table<ChatGroup>()
                .Include(c => c.User).Include(c => c.Reciver).Where(c=>c.Id==id).FirstOrDefaultAsync();
            if (result==null)
            {
                return new ResultDto<ChatGroup>()
                {
                    IsSuccess = false,
                    Message = Alert.Public.NotFound.GetDescription()
                };
            }
            return new ResultDto<ChatGroup>()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
                Data = result
            };
        }

        public async Task<ResultDto<ChatGroup>> GetGroupBy(string token)
        {
            var result = await Table<ChatGroup>()
                .Include(c=>c.User).Include(c=>c.Reciver)
                .FirstOrDefaultAsync(c => c.GroupToken == token);
            if (result == null)
            {
                return new ResultDto<ChatGroup>()
                {
                    IsSuccess = false,
                    Message = Alert.Public.NotFound.GetDescription()
                };
            }
            return new ResultDto<ChatGroup>()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
                Data = result
            };
        }

        public async Task<ResultDto<List<ChatGroup>>> GetUserGroups(long UserId)
        {
            var groups = await Table<ChatGroup>().Include(c => c.Chats)
                .Where(c => c.OwnerId == UserId).OrderByDescending(c => c.createDate).ToListAsync();
            if (groups == null)
            {
                return new ResultDto<List<ChatGroup>>()
                {
                    IsSuccess = false,
                    Message = Alert.Public.NotFound.GetDescription(),
                };
            }
            return new ResultDto<List<ChatGroup>>()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
                Data = groups
            };
        }

        public async Task<ResultDto<ChatGroup>> InsertGroup(CreateGroup_Dto req)
        {
            if (req.ImageFile==null || !FileValidation.IsValidImageFile(req.ImageFile.FileName))
            {
                return new ResultDto<ChatGroup>()
                {
                    IsSuccess = false,
                    Message = "لطفا تصویر گروه را آپلود کنید"
                };
            }
            var imagename = await req.ImageFile.SaveFile("wwwroot/GroupImage");
            var chatgroup = new ChatGroup()
            {
                GroupTitle = req.GroupName,
                OwnerId = req.UserId,
                GroupToken = Guid.NewGuid().ToString(),
                ImageName = imagename,
            };
            Insert(chatgroup);
            await Save();
            await _userGroupsService.InsertInGroup(req.UserId, chatgroup.Id);
            return new ResultDto<ChatGroup>()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
                Data = chatgroup
            };
        }

        public async Task<ResultDto<ChatGroup>> InsertPrivateGroup(long UserId, long ReciverId)
        {
            var group =await Table<ChatGroup>().Include(c=>c.User).Include(c=>c.Reciver)
                .SingleOrDefaultAsync(c => (c.OwnerId == UserId && c.ReciverId == ReciverId) || (c.OwnerId == ReciverId && c.ReciverId == UserId));
            if (group==null)
            {
                var newgroup = new ChatGroup()
                {
                    GroupTitle=$"Chat With{ReciverId}",
                    GroupToken = Guid.NewGuid().ToString(),
                    ImageName="Default.jpg",
                    IsPrivete = true,
                    OwnerId=UserId,
                    ReciverId=ReciverId,
                    
                };
                Insert(newgroup);
                await Save();
               
                return new ResultDto<ChatGroup>()
                {
                    IsSuccess = true,
                    Data =  GetGroupBy(newgroup.Id).Result.Data
                };
            }
            return new ResultDto<ChatGroup>()
            {
                IsSuccess = true,
                Data = group
            };
        }

        public async Task<ResultDto<List<Search_ResultDto>>> Search(string Title,long UserId)
        {
            try
            {
                var result = new List<Search_ResultDto>();
                if (string.IsNullOrWhiteSpace(Title))
                {
                    return new ResultDto<List<Search_ResultDto>>()
                    {
                        IsSuccess = true,
                        Data = result
                    };
                }

                var group =await Table<ChatGroup>().Where(g => g.GroupTitle.Contains(Title) && !g.IsPrivete)
                    .Select(s=>new Search_ResultDto()
                    {
                        ImageName = s.ImageName,
                        IsUser = false,
                        Title=s.GroupTitle,
                        Token=s.GroupToken,
                    }).ToListAsync();

                var users = await Table<User>().Where(g => g.UserName.Contains(Title)&& g.Id !=UserId)
                .Select(s => new Search_ResultDto()
                {
                    ImageName = s.Avatar,
                    IsUser = true,
                    Title = s.UserName,
                    Token = s.Id.ToString(),
                }).ToListAsync();

                result.AddRange(users);
                result.AddRange(group);

                return new ResultDto<List<Search_ResultDto>>()
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception)
            {

                return new ResultDto<List<Search_ResultDto>>()
                {
                    IsSuccess = false,
                    Message = Alert.Public.ServerException.GetDescription()
                };
            }
        }
    }
    public class CreateGroup_Dto
    {
        public string GroupName { get; set; }
        public long UserId { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class Search_ResultDto
    {
        public string Title { get; set; }
        public string Token { get; set; }
        public string ImageName { get; set; }
        public bool IsUser { get; set; }
    }
}
