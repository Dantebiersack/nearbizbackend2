namespace nearbizbackend.DTOs
{
    public record PersonalReadDto(int IdPersonal, int IdUsuario, int IdNegocio, string? RolEnNegocio, DateTime FechaRegistro, bool Estado);
    public record PersonalCreateDto(int IdUsuario, int IdNegocio, string? RolEnNegocio);
    public record PersonalUpdateDto(int IdUsuario, int IdNegocio, string? RolEnNegocio);
}