using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proeva.DTOs;
using proeva.Services;

namespace proeva.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult RegisterUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUsuario(RegisterUsuarioDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var (success, error) = await _authService.RegisterUsuarioAsnc(registerDto);
            
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(registerDto);
            }

            return RedirectToAction("Usuarios");
        }

        [HttpGet]
        public IActionResult Usuarios()
        {
            return View();
        }
    }
}