using Microsoft.EntityFrameworkCore;
using proeva.Dto;
using proeva.Models;

namespace proeva.Services;

public class ClienteService : IClienteService
{
    private readonly GestionComprasContext _context;

    public ClienteService(GestionComprasContext context)
    {
        _context = context;
    }

    public async Task<ClienteDto> Crear(ClienteDto clienteDto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == clienteDto.Correo);
        if (usuario == null)
        {
            var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cliente");
            if (rolCliente == null)
            {
                throw new Exception("No se encontró el rol 'Cliente'");
            }

            usuario = new Usuario
            {
                Nombre = clienteDto.Nombre,
                Correo = clienteDto.Correo,
                ContrasenaHash = "temp_password", 
                RolId = rolCliente.Id
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        var cliente = new Cliente
        {
            Nombre = clienteDto.Nombre,
            Direccion = clienteDto.Direccion,
            UsuarioId = usuario.Id
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        clienteDto.Id = cliente.Id;
        return clienteDto;
    }

    public async Task<ClienteDto> ObtenerPorId(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
        {
            return null;
        }

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Direccion = cliente.Direccion,
            Correo = cliente.Usuario.Correo
        };
    }

    public async Task<List<ClienteDto>> ObtenerTodos()
    {
        return await _context.Clientes
            .Include(c => c.Usuario)
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Direccion = c.Direccion,
                Correo = c.Usuario.Correo
            }).ToListAsync();
    }

    public async Task Actualizar(int id, ClienteDto clienteDto)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
        }

        cliente.Nombre = clienteDto.Nombre;
        cliente.Direccion = clienteDto.Direccion;

        // Actualizar correo del usuario asociado
        if (cliente.Usuario != null)
        {
            cliente.Usuario.Correo = clienteDto.Correo;
        }

        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }
}