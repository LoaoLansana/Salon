using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salao.Repository.Models.Account.Views
{
    public class ManageRolePermissionsViewModel
    {
        public string RoleID { get; set; }
        public IList<RolePermissionsViewModel> RolePermissions { get; set; }
    }

    public class RolePermissionsViewModel
    {
        public string PermissionName { get; set; }
        public bool Selected { get; set; }
    }
}
