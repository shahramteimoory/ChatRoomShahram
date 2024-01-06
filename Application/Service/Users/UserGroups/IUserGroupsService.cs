using Application.Interface.Context;
using Common;
using Common.Dto;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Service.Users.UserGroups
{
    public interface IUserGroupsService
    {
        Task<ResultDto<List<UserGroup_Dto>>> GetUserGroups(long UserId);
        Task<ResultDto> InsertInGroup(long UserId,long GroupId);
        Task<ResultDto> IsUserInGroup(long UserId, long GroupId);
        Task<ResultDto> IsUserInGroup(long UserId, string Token);
        Task<ResultDto<List<string>>> GetUserIds(long GroupId);
        Task<ResultDto> InsertInGroup(List<long> UserIds, long GroupId);
    }
    public class UserGroupsService : BaseService, IUserGroupsService
    {
        public UserGroupsService(IDataBaseContext context) : base(context) 
        { }

        public async Task<ResultDto<List<UserGroup_Dto>>> GetUserGroups(long UserId)
        {
            try
            {
                var result = await Table<UserGroup>().Include(g => g.ChatGroup.Chats).Include(c => c.ChatGroup.Reciver).Include(c => c.ChatGroup.User)
                    .Where(g => g.UserId == UserId && !g.ChatGroup.IsPrivete).ToListAsync();
                   
                var model =new  List<UserGroup_Dto>();
                foreach (var group in result)
                {
                    if (group.ChatGroup.ReciverId!=null)
                    {
                        if (group.ChatGroup.ReciverId==UserId)
                        {
                            model.Add(new UserGroup_Dto
                            {
                                ImageName = group.ChatGroup.User.Avatar,
                                GroupName=group.ChatGroup.User.UserName,
                                Token=group.ChatGroup.GroupToken,
                                LastChat=group.ChatGroup.Chats.OrderByDescending(c=>c.Id).FirstOrDefault()
                            });
                        }
                        else
                        {
                            model.Add(new UserGroup_Dto
                            {
                                ImageName = group.ChatGroup.Reciver.Avatar,
                                GroupName = group.ChatGroup.Reciver.UserName,
                                Token = group.ChatGroup.GroupToken,
                                LastChat = group.ChatGroup.Chats.OrderByDescending(c => c.Id).FirstOrDefault()
                            });
                        }
                    }
                    else
                    {
                        model.Add(new UserGroup_Dto
                        {
                            ImageName = group.ChatGroup.ImageName,
                            GroupName = group.ChatGroup.GroupTitle,
                            Token = group.ChatGroup.GroupToken,
                            LastChat = group.ChatGroup.Chats.OrderByDescending(c => c.Id).FirstOrDefault()
                        });
                    }
                }

                return new ResultDto<List<UserGroup_Dto>>()
                {
                    IsSuccess = true,
                    Data = model,
                    Message = Alert.Public.Success.GetDescription()
                };
            }
            catch 
            {

                return new ResultDto<List<UserGroup_Dto>>()
                {
                    IsSuccess = false,
                    Message = Alert.Public.ServerException.GetDescription()
                };
            }
        }

        public async Task<ResultDto<List<string>>> GetUserIds(long GroupId)
        {
            var ids=await Table<UserGroup>().Where(g=>g.GroupId==GroupId).Select(s=>s.UserId.ToString()).ToListAsync();
            return new ResultDto<List<string>>()
            {
                IsSuccess = true,
                Data = ids,
            };
        }

        public async Task<ResultDto> InsertInGroup(long UserId, long GroupId)
        {
            try
            {
                var model = new UserGroup()
                {
                    GroupId = GroupId,
                    UserId = UserId
                };
                Insert(model);
                await Save();
                return new ResultDto() 
                {
                    IsSuccess = true,
                    Message=Alert.Public.Success.GetDescription(),
                };
            }
            catch
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = Alert.Public.ServerException.GetDescription()
                };
            }
        }

        public async Task<ResultDto> InsertInGroup(List<long> UserIds, long GroupId)
        {
            try
            {
                foreach (var UserId in UserIds)
                {
                    var model = new UserGroup()
                    {
                        GroupId = GroupId,
                        UserId = UserId
                    };
                    Insert(model);
                }
                await Save();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = Alert.GetAddAlert(Alert.Entity.User)
                };

            }
            catch
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = Alert.Public.ServerException.GetDescription(),
                };
            }
        }

        public async Task<ResultDto> IsUserInGroup(long UserId, long GroupId)
        {
            var result =await Table<UserGroup>().AnyAsync(g => g.GroupId == GroupId && g.UserId == UserId);
            if (result == false)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = Alert.Public.NotFound.GetDescription(),
                };
            }
            return new ResultDto()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
            };
        }

        public async Task<ResultDto> IsUserInGroup(long UserId, string Token)
        {
            var result = await Table<UserGroup>().Include(f=>f.ChatGroup)
                .AnyAsync(g => g.ChatGroup.GroupToken == Token && g.UserId == UserId);
            if (result == false)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = Alert.Public.NotFound.GetDescription(),
                };
            }
            return new ResultDto()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
            };
        }
    }

    public class UserGroup_Dto
    {
        public string GroupName { get; set; }
        public string ImageName { get; set; }
        public string Token { get; set; }
        public Chat LastChat { get; set; }

    }
}
