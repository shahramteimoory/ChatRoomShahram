using Application.Interface.Context;
using Common;
using Common.Dto;
using Common.Security;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users
{
    public interface IUserService
    {
        Task<ResultDto> IsUserExcist(string UserName);
        Task<ResultDto> IsUserExcist(long UserId);
        Task<ResultDto> RegisterUser(Register_Dto req);
        Task<ResultDto<User>> Login(Login_Dto req);
    }
    public class UserService:BaseService, IUserService
    {
        public UserService(IDataBaseContext context) : base(context) { }

        public async Task<ResultDto> IsUserExcist(string UserName)
        {
            bool result = await Table<User>().AnyAsync(u=>u.UserName==UserName.ToLower());
            if (result == true)
            {
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "این نام کاربری وجود دارد"
                };
            }
            return new ResultDto()
            {
                IsSuccess = false,
            };
        }

        public async Task<ResultDto> IsUserExcist(long UserId)
        {
            bool result = await Table<User>().AnyAsync(u => u.Id == UserId);
            if (result == true)
            {
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "این نام کاربری وجود دارد"
                };
            }
            return new ResultDto()
            {
                IsSuccess = false,
            };
        }

        public async Task<ResultDto<User>> Login(Login_Dto req)
        {
            var user = await Table<User>().SingleOrDefaultAsync(u=>u.UserName==req.UserName.ToLower());
            if (user==null)
            {
                return new ResultDto<User>()
                {
                    IsSuccess = false,
                    Message = Alert.Public.NotFound.GetDescription()
                };
            }
            var password = req.Password.EncodePasswordMd5();
            if (password!=user.Password)
            {
                return new ResultDto<User>()
                {
                    IsSuccess = false,
                    Message = "کلمه عبور اشتباه وارد شده"
                };
            }
            return new ResultDto<User>()
            {
                IsSuccess = true,
                Message = Alert.Public.Success.GetDescription(),
                Data = user
            };
        }

        public async Task<ResultDto> RegisterUser(Register_Dto req)
        {
            var exci = await IsUserExcist(req.UserName);
            if (exci.IsSuccess==true)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = exci.Message
                };
            }
            if (req.Password != req.RePassword)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message="کلمه عبور و تکرار آن برابر نیست"
                };
            }
            var password=req.Password.EncodePasswordMd5();
            var User = new User()
            {
                Password = password,
                Avatar="Default.jpg",
                UserName=req.UserName.ToLower(),
            };
            Insert(User);
            await Save();
            return new ResultDto()
            {
                IsSuccess = true,
                Message = Alert.GetAddAlert(Alert.Entity.User)
            };
        }
    }

    public class Register_Dto
    {
        [Required(ErrorMessage ="لطفا نام کاربری را وارد کنید")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "لطفا کلمه عبور را وارد کنید")]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="کلمه عبور و تکرار ان برابر نیست")]
        public string RePassword { get; set; }

    }
    public class Login_Dto
    {
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "لطفا کلمه عبور را وارد کنید")]
        public string Password { get; set; }
    }
}
