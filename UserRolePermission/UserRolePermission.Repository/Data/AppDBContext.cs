using Microsoft.EntityFrameworkCore;
using UserRolePermission.Repository.Models;

namespace UserRolePermission.Repository.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<UserDb> Users { get; set; }
        public DbSet<StatusDb> UserStatus { get; set; }
        public DbSet<RoleDb> Roles { get; set; }
        public DbSet<ScreenDb> Screens { get; set; }
        public DbSet<ScreenActionDb> ScreenActions { get; set; }
        public DbSet<RolePermissionDb> RolePermissions { get; set; }
        public DbSet<UserRoleDb> UserRoles { get; set; }
        public DbSet<UserPermissionOverrideDb> UserPermisssionOverride { get; set; }
        public DbSet<MenuStructureDb> MenuStructures { get; set; }
    }

}
