namespace nearbizbackend.DTOs
{
    public record MembresiaReadDto(int IdMembresia, decimal? PrecioMensual, int IdNegocio, bool Estado);
    public record MembresiaCreateDto(decimal? PrecioMensual, int IdNegocio);
    public record MembresiaUpdateDto(decimal? PrecioMensual, int IdNegocio);
}
