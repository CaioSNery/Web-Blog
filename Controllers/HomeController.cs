using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Atributte;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        //health check - endpoint padrao-root/pra saber se a API está Online.

        [HttpGet("")] //podem ter outras convenções que mudam o nome da route .

        public IActionResult Index()
        {
            return Ok("Api Running..!");
        }
    }
}