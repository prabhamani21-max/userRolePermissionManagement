using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class UserDto
    {

        public long Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; } 
        public int StatusId { get; set; }
        public string ContactNo { get; set; }
        public int DefaultRoleId { get; set; }


    }
}