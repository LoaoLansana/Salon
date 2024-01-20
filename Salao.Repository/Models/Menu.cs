using System;
using System.Collections.Generic;

namespace Salao.Repository.Models
{
    public partial class Menu
    {
        public Menu()
        {
            InverseParentMenu = new HashSet<Menu>();
            MenuRoles = new HashSet<MenuRole>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
        public string? Route { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? ParentMenuId { get; set; }

        public virtual Menu? ParentMenu { get; set; }
        public virtual ICollection<Menu> InverseParentMenu { get; set; }
        public virtual ICollection<MenuRole> MenuRoles { get; set; }
    }
}
