namespace nearbizbackend.DTOs
{
    public record ServicioReadDto(int IdServicio, int IdNegocio, string NombreServicio, string? Descripcion, decimal? Precio, int? DuracionMinutos, bool Estado);
    public record ServicioCreateDto(int IdNegocio, string NombreServicio, string? Descripcion, decimal? Precio, int? DuracionMinutos);
    public record ServicioUpdateDto(int IdNegocio, string NombreServicio, string? Descripcion, decimal? Precio, int? DuracionMinutos);
}
