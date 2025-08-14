using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewModels.Account
{
    public class UploadImageViewModel
    {
        [Required(ErrorMessage = "Imagem é Inválida")]
        public string Base64Image { get; set; }
    }
}