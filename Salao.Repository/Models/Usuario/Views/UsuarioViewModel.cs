using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Salao.Repository.Models.Usuario.Views
{
    public class UsuarioViewModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Informe o Nome de Usuário")]
        //[RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Nome de Usuário inválido. Use apenas letras, números e underscores.")]
        [Display(Name = "Nome de Usuário")]
        public string FullName { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        public string Password { get; set; }

        public IEnumerable<string> Roles { get; set; }

    }
}
