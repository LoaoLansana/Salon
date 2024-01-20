using System.ComponentModel.DataAnnotations;

namespace Salao.Repository.Models.Account.Views
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Antiga")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme a Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação de senha não correspondem.")]
        public string ConfirmNewPassword { get; set; }
    }
}
