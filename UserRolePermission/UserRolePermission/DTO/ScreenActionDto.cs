using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class ScreenActionDto
    {
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string Key { get; set; }
        public int ScreenId { get; set; }
        public int StatusId { get; set; }
    }
}