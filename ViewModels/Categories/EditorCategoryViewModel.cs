using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models.ViewModels.Categories
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "Nome Obrigatorio")] //validações DataAnnotations
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
    }
}