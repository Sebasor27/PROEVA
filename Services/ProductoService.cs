using Microsoft.EntityFrameworkCore;
using proeva.Dto;
using proeva.Models;

namespace proeva.Services;

public class ProductoService : IProductoService
{
    private readonly GestionComprasContext _context;

    public ProductoService(GestionComprasContext context)
    {
        _context = context;
    }

    public async Task<ProductoDto> Crear(ProductoDto productoDto)
    {
        var producto = new Producto
        {
            Nombre = productoDto.Nombre,
            Precio = productoDto.Precio
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        // Retornar el DTO con el ID generado
        productoDto.Id = producto.Id;
        return productoDto;
    }

    public async Task<ProductoDto> ObtenerPorId(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        
        if (producto == null)
        {
            return null;
        }

        return new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio
        };
    }

    public async Task<List<ProductoDto>> ObtenerTodos()
    {
        return await _context.Productos
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio
            }).ToListAsync();
    }

    public async Task Actualizar(int id, ProductoDto productoDto)
    {
        var producto = await _context.Productos.FindAsync(id);
        
        if (producto == null)
        {
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
        }

        producto.Nombre = productoDto.Nombre;
        producto.Precio = productoDto.Precio;

        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        
        if (producto == null)
        {
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
    }
}