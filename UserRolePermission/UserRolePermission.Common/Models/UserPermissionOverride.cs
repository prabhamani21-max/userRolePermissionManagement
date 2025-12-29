using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRolePermission.Common.Models
{
    public class UserPermissionOverride
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int ActionId { get; set; }
        public bool IsDenied { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public virtual Status Status { get; set; }
        public virtual ScreenAction Action { get; set; }
        public virtual User User { get; set; }
    }
}
