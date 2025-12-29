using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRolePermission.Common.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int StatusId { get; set; }
        public string ContactNo { get; set; }
        public int DefaultRoleId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public virtual Status Status { get; set; }
        public virtual Role DefaultRole { get; set; }
    }
}
