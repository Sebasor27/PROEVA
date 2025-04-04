using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using proeva.DTOs;
using proeva.Models;
using System.Security.Claims;

namespace proeva.Services
{
    public interface IAuthService
    {
        Task<(bool success, string error)> RegisterAsync(RegisterDto registerDto);
        Task<bool> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
        Task<(bool success, string error)> RegisterUsuarioAsnc(RegisterUsuarioDto registerDto);

    }

    public class AuthService : IAuthService
    {
        private readonly GestionComprasContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            GestionComprasContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<(bool success, string error)> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var correoNormalizado = registerDto.Correo.Trim().ToLower();
                var existeUsuario = await _context.Usuarios
                    .AnyAsync(u => EF.Functions.Like(u.Correo, correoNormalizado));

                _logger.LogInformation($"Verificación de correo: {existeUsuario} para {correoNormalizado}");

                if (existeUsuario)
                {
                    var usuarioExistente = await _context.Usuarios
                        .FirstOrDefaultAsync(u => EF.Functions.Like(u.Correo, correoNormalizado));

                    _logger.LogWarning($"Intento de registro con correo existente: {usuarioExistente?.Correo}");
                    return (false, "El correo ya está registrado");
                }

                var rolCliente = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre == "Cliente");

                if (rolCliente == null)
                {
                    _logger.LogError("Rol 'Cliente' no encontrado en la base de datos");
                    return (false, "Error en la configuración del sistema");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var usuario = new Usuario
                    {
                        Nombre = registerDto.Nombre.Trim(),
                        Correo = registerDto.Correo.Trim(),
                        ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Contrasena),
                        RolId = rolCliente.Id
                    };

                    await _context.Usuarios.AddAsync(usuario);
                    await _context.SaveChangesAsync();

                    var cliente = new Cliente
                    {
                        Nombre = registerDto.Nombre.Trim(),
                        Direccion = "Por definir",
                        UsuarioId = usuario.Id
                    };

                    await _context.Clientes.AddAsync(cliente);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    _logger.LogInformation($"Usuario registrado exitosamente: {usuario.Correo}");
                    return (true, null);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error durante el registro");
                    return (false, "Error durante el registro. Intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en RegisterAsync");
                return (false, "Error inesperado. Contacte al administrador.");
            }
        }
        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == loginDto.Correo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Contrasena, usuario.ContrasenaHash) || usuario.Rol.Nombre == "Cliente")
            {
                return false;
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim(ClaimTypes.Email, usuario.Correo),
        new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = loginDto.RememberMe });

            return true;
        }

        public async Task<(bool success, string error)> RegisterUsuarioAsyc(RegisterUsuarioDto registerDto)
        {
            try
            {
                var correoNormalizado = registerDto.Correo.Trim().ToLower();
                var existeUsuario = await _context.Usuarios
                    .AnyAsync(u => EF.Functions.Like(u.Correo, correoNormalizado));

                if (existeUsuario)
                {
                    return (false, "El correo ya está registrado");
                }

                var rolUsuario = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre == "Usuario");

                if (rolUsuario == null)
                {
                    return (false, "Rol de usuario no configurado");
                }

                var usuario = new Usuario
                {
                    Nombre = registerDto.Nombre.Trim(),
                    Correo = registerDto.Correo.Trim(),
                    ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Contrasena),
                    RolId = rolUsuario.Id
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return (false, "Error al registrar usuario");
            }
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public Task<(bool success, string error)> RegisterUsuarioAsnc(RegisterUsuarioDto registerDto)
        {
            throw new NotImplementedException();
        }
    }
}