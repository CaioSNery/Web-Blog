using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using BlogApi.Models.ViewModels;
using BlogApi.Models.ViewModels.Categories;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("v1")] // Rota fixa na url / v1

    public class CategoryController : ControllerBase
    {
        // [HttpGet("v2/categories")] //versionamento da API-novas versoes nao quebrem o codigo antigo.
        // public IActionResult Get2([FromServices] BlogDataContext context)
        // {
        //     var categories = context.Categories.ToList();
        //     return Ok(categories);
        // }


        [HttpGet("categories")] //padronização da nomeação dos endpoints. pode ser = userRoles/user-roles.
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>(error: "05x11 - Falha interna do servidor"));
            }

        }

        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>(error: "Não Encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>(error: "Falha interna no servidor"));
            }
        }

        [HttpPost("categories")]
        public async Task<IActionResult> PostAsyn([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
        {

            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new ResultViewModel<Category>(error: $"05xC10 - Não foi possivel incluir Categoria{ex.Message}")); // 05xC10 importante incluir o numero do error, para melhor tratar posteriormente.
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>(error: "05x11 - Falha interna do servidor"));
            }
        }

        [HttpPut("categories/{id:int}")]
        public async Task<IActionResult> PutAsyn([FromRoute] int id, [FromBody] Category category, [FromServices] BlogDataContext context)
        {
            try
            {
                var category1 = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category1 == null) return NotFound(new ResultViewModel<Category>(error: "Não Encontrado"));

                category1.Name = category.Name;
                category1.Posts = category.Posts;
                category1.Slug = category.Slug;

                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category1));
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new ResultViewModel<Category>(error: $"05xC12 - Não foi possivel alterar Categoria{ex.Message}")); //  importante incluir o numero do error, para melhor tratar posteriormente.
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>(error: $"05x13 - Falha interna do servidor{e.Message}"));
            }

        }

        [HttpDelete("categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (categories == null) return NotFound(new ResultViewModel<Category>(error: "Não Encontrado"));

                context.Categories.Remove(categories);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(categories));
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new ResultViewModel<Category>(error: $"Error ao tentar acessar o Banco de Dados{ex.Message}")); //  importante incluir o numero do error, para melhor tratar posteriormente.
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>(error: $"05x15 - Falha interna do servidor{ex.Message}"));
            }
        }


    }
}