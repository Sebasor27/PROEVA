using proeva.Dto;

namespace proeva.Services;

public interface IClienteService
{
    Task<List<ClienteDto>> ObtenerTodos();
    Task<ClienteDto> ObtenerPorId(int id);
    Task<ClienteDto> Crear(ClienteDto clienteDto);
    Task Actualizar(int id, ClienteDto clienteDto);
    Task Eliminar(int id);
}