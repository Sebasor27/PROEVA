using proeva.Dto;

namespace proeva.Services;
public interface IProductoService
{
    Task<List<ProductoDto>> ObtenerTodos();
    Task<ProductoDto> ObtenerPorId(int id);
    Task<ProductoDto> Crear(ProductoDto productoDto);
    Task Actualizar(int id, ProductoDto productoDto);
    Task Eliminar(int id);
}