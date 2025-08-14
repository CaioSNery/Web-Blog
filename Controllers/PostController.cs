using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Blog.ViewModels.Posts;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1")]
    public class PostController : ControllerBase
    {
        private readonly BlogDataContext _context;
        public PostController(BlogDataContext context)
        {
            _context = context;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPostAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 5)

        {
            try
            {
                var count = await _context.Posts.AsNoTracking().CountAsync();
                var posts = await _context.Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Select(x => new ListPostViewModels//Limitando informações atraves do .Select();
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name}({x.Author.Email})"
                }

                )
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new //Tipo dynamic por que esta criando objt anonimo
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Post>(error: $"Falha interna do servidor{ex.Message}"));
            }


        }

        [HttpGet("posts/{id:int}")]
        public async Task<IActionResult> GetPostById([FromRoute] int id)
        {
            try
            {
                var post = await _context.Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .ThenInclude(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<string>(error: "Post não encontrado"));
                return Ok(new ResultViewModel<Post>(post));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Post>(error: $"Falha Interna do servidor{ex.Message}"));
            }
        }


    }
}