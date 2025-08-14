using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewModels.Account
{
    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Email Inválido!")]
        [Required(ErrorMessage = "Informe o Email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha!")]
        public string Password { get; set; }
    }
}