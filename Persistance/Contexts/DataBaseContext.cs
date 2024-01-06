using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Context;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Persistance.Contexts
{
    public class DataBaseContext : DbContext, IDataBaseContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserGroup> userGroups {  get; set; }

        public DbConnection GetDbConnection(DatabaseFacade databaseFacade)
        {
            var relationalConnection = databaseFacade.GetDbConnection();
            return relationalConnection;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User)
                .WithMany(u => u.Chats)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserGroup>()
            .HasOne(c => c.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }


}
