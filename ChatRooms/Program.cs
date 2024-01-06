using Application.Interface.Context;
using Application.Interface.FacadPatterns;
using Application.Service.Chats.FacadPatterns;
using Application.Service.Roles.FacadPatterns;
using Application.Service.Users.FacadPatterns;
using Application.Service.Users.UserGroups;
using ChatRooms.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistance.Contexts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IDataBaseContext, DataBaseContext>();
builder.Services.AddScoped<IUserFacad, UserFacad>();
builder.Services.AddScoped<IRoleFacad, RoleFacad>();
builder.Services.AddScoped<IChatsFacad, ChatsFacad>();
builder.Services.AddScoped<IUserGroupsService, UserGroupsService>();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(optioN =>
{
    optioN.LoginPath = "/auth";
    optioN.LoginPath = "/auth/logout";
    optioN.ExpireTimeSpan= TimeSpan.FromDays(30);
});
// Add services to the container.
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/Chat");
app.Run();
