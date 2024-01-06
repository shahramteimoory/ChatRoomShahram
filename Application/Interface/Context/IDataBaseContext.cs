using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Context
{
    public interface IDataBaseContext
    {
        DbSet<Chat> Chats { get; set; }
        DbSet<ChatGroup> ChatGroups { get; set; }
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<UserRole> UserRoles { get; set;}
        DbSet<Role> Roles { get; set; }
        DbSet<UserGroup> userGroups { get; set; }

        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        DbConnection GetDbConnection(DatabaseFacade databaseFacade);
    }
}
