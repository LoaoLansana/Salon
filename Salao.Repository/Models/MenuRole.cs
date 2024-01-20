using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Salao.Repository.Models
{
    public partial class MenuRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string RoleId { get; set; }

        public Menu Menu { get; set; }
        public IdentityRole Role { get; set; }
    }
}
