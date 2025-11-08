namespace nearbizbackend.DTOs
{
    public record UsuarioReadDto(int IdUsuario, string Nombre, string Email, int IdRol, DateTime FechaRegistro, bool Estado, string? Token);
    public record UsuarioCreateDto(string Nombre, string Email, string ContrasenaHash, int IdRol, string? Token);
    public record UsuarioUpdateDto(string Nombre, string Email, int IdRol, string? Token);
}
