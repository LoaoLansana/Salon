using Renavam.Repository.Models;

namespace Salao.Repository.Models.Menus.View
{
    public class MenuRoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Route { get; set; }
        public List<MenuRoleModel> Roles { get; set; }
        public int? ParentMenuId { get; set; }
        public Menu ParentMenu { get; set; }
        public ICollection<Menu> SubMenus { get; set; }
    }

    public class MenuRoleModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
