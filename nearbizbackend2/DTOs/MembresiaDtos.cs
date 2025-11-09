namespace nearbizbackend.DTOs
{
    public record MembresiaReadDto(
        int IdMembresia,
        decimal? PrecioMensual,
        int IdNegocio,
        bool Estado,
        DateTime? UltimaRenovacion
    );

    public record MembresiaCreateDto(
        decimal? PrecioMensual,
        int IdNegocio,
        DateTime? UltimaRenovacion
    );

    public record MembresiaUpdateDto(
        decimal? PrecioMensual,
        int IdNegocio,
        DateTime? UltimaRenovacion
    );

    public record MembresiaAdminRowDto(
        int IdMembresia,
        int IdNegocio,
        string NombreNegocio,
        decimal? PrecioMensual,
        bool Estado,
        DateTime? UltimaRenovacion
    );
}
