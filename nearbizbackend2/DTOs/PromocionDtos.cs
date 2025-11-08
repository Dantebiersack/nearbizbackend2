namespace nearbizbackend.DTOs
{
    public record PromocionReadDto(int IdPromocion, int IdNegocio, string Titulo, string? Descripcion, DateOnly FechaInicio, DateOnly FechaFin, bool Estado);
    public record PromocionCreateDto(int IdNegocio, string Titulo, string? Descripcion, DateOnly FechaInicio, DateOnly FechaFin);
    public record PromocionUpdateDto(int IdNegocio, string Titulo, string? Descripcion, DateOnly FechaInicio, DateOnly FechaFin);
}