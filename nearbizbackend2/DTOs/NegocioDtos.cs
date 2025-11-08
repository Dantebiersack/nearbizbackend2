namespace nearbizbackend.DTOs
{
    public record NegocioReadDto(int IdNegocio, int IdCategoria, int? IdMembresia, string Nombre, string? Direccion,
        decimal? CoordenadasLat, decimal? CoordenadasLng, string? Descripcion, string? TelefonoContacto,
        string? CorreoContacto, string? HorarioAtencion, bool Estado, string? LinkUrl);

    public record NegocioCreateDto(int IdCategoria, int? IdMembresia, string Nombre, string? Direccion,
        decimal? CoordenadasLat, decimal? CoordenadasLng, string? Descripcion, string? TelefonoContacto,
        string? CorreoContacto, string? HorarioAtencion, string? LinkUrl);

    public record NegocioUpdateDto(int IdCategoria, int? IdMembresia, string Nombre, string? Direccion,
        decimal? CoordenadasLat, decimal? CoordenadasLng, string? Descripcion, string? TelefonoContacto,
        string? CorreoContacto, string? HorarioAtencion, string? LinkUrl);
}
