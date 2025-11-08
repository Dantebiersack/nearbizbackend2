namespace nearbizbackend.DTOs
{
    public record ClienteReadDto(int IdCliente, int IdUsuario, DateTime FechaRegistro, bool Estado);
    public record ClienteCreateDto(int IdUsuario);
    public record ClienteUpdateDto(int IdUsuario);
}
