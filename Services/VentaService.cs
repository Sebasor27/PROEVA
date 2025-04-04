using Microsoft.EntityFrameworkCore;
using proeva.Dto;
using proeva.Models;
using proeva.Services;
using proeva.ViewModels;

namespace proeva.Services;

public class VentaService : IVentaService
{
    private readonly GestionComprasContext _context;
    private readonly IProductoService _productoService;

    public VentaService(GestionComprasContext context, IProductoService productoService)
    {
        _context = context;
        _productoService = productoService;
    }

    public async Task<VentaViewModel> PrepararVentaViewModel()
    {
        try
        {
            var productos = await _productoService.ObtenerTodos();
            var clientes = await _context.Clientes
                .Include(c => c.Usuario)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Direccion = c.Direccion,
                    Correo = c.Usuario.Correo
                }).ToListAsync();

            var metodosPago = await _context.MetodosPagos.ToListAsync();

            return new VentaViewModel
            {
                ProductosDisponibles = productos ?? new List<ProductoDto>(),
                Clientes = clientes ?? new List<ClienteDto>(),
                MetodosPago = metodosPago ?? new List<MetodosPago>(),
                Pedido = new PedidoDto()
            };
        }
        catch (Exception ex)
        {
            // Log the error and return empty view model
            return new VentaViewModel();
        }
    }
    public async Task<PedidoDto> ProcesarVenta(PedidoDto pedidoDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var pedido = new Pedido
            {
                ClienteId = pedidoDto.ClienteId,
                MetodoPagoId = pedidoDto.MetodoPagoId,
                Estado = pedidoDto.Estado,
                Fecha = DateTime.Now
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            foreach (var detalle in pedidoDto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado");

                var detallePedido = new DetallesPedido
                {
                    PedidoId = pedido.Id,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario
                };

                _context.DetallesPedidos.Add(detallePedido);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            pedidoDto.Id = pedido.Id;
            return pedidoDto;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ProductoDto>> BuscarProductos(string term)
    {
        return await _context.Productos
            .Where(p => p.Nombre.Contains(term) || p.Id.ToString() == term)
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio
            })
            .ToListAsync();
    }

    public async Task<List<ClienteDto>> BuscarClientes(string term)
    {
        return await _context.Clientes
            .Include(c => c.Usuario)
            .Where(c => c.Nombre.Contains(term) ||
                       c.Usuario.Correo.Contains(term) ||
                       c.Id.ToString() == term)
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Direccion = c.Direccion,
                Correo = c.Usuario.Correo
            })
            .ToListAsync();
    }

    public async Task<ClienteDto> ObtenerCliente(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null) return null;

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Direccion = cliente.Direccion,
            Correo = cliente.Usuario.Correo
        };
    }
}