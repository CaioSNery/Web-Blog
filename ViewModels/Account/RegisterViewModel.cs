using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nome Obrigatorio")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email Obrigatorio")]
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string Email { get; set; }
    }
}