using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class ScreenDto
    {
        public int Id { get; set; }
        public string ScreenName { get; set; }
        public string Key { get; set; }
        public int StatusId { get; set; }
    }
}