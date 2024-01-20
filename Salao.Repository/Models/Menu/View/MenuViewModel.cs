using Renavam.Repository.Models;
using System.ComponentModel.DataAnnotations;

namespace Salao.Repository.Models.Menus.View
{
    public class MenuViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [StringLength(255, MinimumLength = 1)]
        [Display(Name = "Ícone")]
        public string? Icon { get; set; }
        [Display(Name = "Rota")]
        [StringLength(255, MinimumLength = 1)]
        public string? Route { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<string>? SelectedRoleIds { get; set; }
        [Display(Name = "Menu Pai")]
        public int? ParentMenuId { get; set; }
        public Menu? ParentMenu { get; set; }
        public ICollection<Menu>? SubMenus { get; set; }
    }
}
