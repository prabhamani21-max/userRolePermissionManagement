namespace UserRolePermission.Service.Interface
{
    public interface ICurrentUser
    {
        public long UserId { get; set; }
        List<string> Roles { get; set; }
        int RoleId { get; set; }
    }
}
