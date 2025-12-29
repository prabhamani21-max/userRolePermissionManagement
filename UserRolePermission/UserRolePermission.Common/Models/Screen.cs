using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRolePermission.Common.Models
{
    public class Screen
    {
        public int Id { get; set; }
        public string ScreenName { get; set; }
        public string Key { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public virtual Status Status { get; set; }
    }
}
