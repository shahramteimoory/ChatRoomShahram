using Application.Interface.FacadPatterns;
using Application.Service.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace ChatRooms.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserFacad _userFacad;
        public AuthController(IUserFacad userFacad)
        {
            _userFacad=userFacad;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register_Dto req)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", req);
            }
          var result=await  _userFacad.UserService.RegisterUser(req);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(req.UserName,result.Message);
                return View("Index",req);
            }

            return Redirect("/auth");

        }
        public async Task<IActionResult> Login(Login_Dto req)
        {
            if (!ModelState.IsValid) 
            {
                return View("index",req);
            }
            var user=await _userFacad.UserService.Login(req);
            if (!user.IsSuccess)
            {
                ModelState.AddModelError(req.UserName, user.Message);
                return View("index", req);
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Data.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Data.UserName.ToString())
            };
            var identity= new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            var principal=new ClaimsPrincipal(identity);
            var propertis = new AuthenticationProperties()
            {
                IsPersistent = true,
            };
            await HttpContext.SignInAsync(principal);
            return Redirect("/");
        }
    }
}
