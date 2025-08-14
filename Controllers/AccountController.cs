using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Account;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Blog.Controllers
{

    [ApiController]
    [Route("v1")]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        public AccountController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("account")]
        public async Task<IActionResult> Post(
    [FromBody] RegisterViewModel model,
    [FromServices] BlogDataContext context,
    [FromServices] EmailService service)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            // Verifica se email já existe
            var existingUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (existingUser != null)
                return BadRequest(new ResultViewModel<string>("Email já está cadastrado"));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")

            };

            var password = PasswordGenerator.Generate(length: 25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                service.Send(
                    user.Name,
                    user.Email,
                    subject: "Welcome to blog",
                    body: $"Senha é {password}",
                    fromName: "Equipe Balta.io",
                    fromEmail: "caionery40@gmail.com"
                );



                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>($"Erro interno: {ex.InnerException?.Message ?? ex.Message}"));
            }

        }


        [HttpPost("account/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model,
        BlogDataContext context)//Poderia fazer [FromService] TokenService tokenService
        {

            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>(error: "Usuario ou senha inválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>(error: "Usuario ou senha inválidos"));

            try
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>(error: "Falha interna"));

            }

        }

        [Authorize]
        [HttpPost("account/upload-image")]
        public async Task<IActionResult> UploadImage(
            [FromBody] UploadImageViewModel model,
            [FromServices] BlogDataContext context)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"data:imageV[a-z]+;base64").Replace(model.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>(error: $"Error Interno{ex.Message}"));

            }

            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user == null) return NotFound(new ResultViewModel<User>(error: "Usuário não encontrado"));

            user.Image = $"https://localhost:0000/images/{fileName}";

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>(error: $"Falha Interna : {ex.Message}"));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso !", null));


        }
    }
}