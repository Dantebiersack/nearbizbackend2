namespace nearbizbackend.DTOs
{
    public record RolReadDto(int IdRol, string RolNombre, bool Estado);
    public record RolCreateDto(string RolNombre);
    public record RolUpdateDto(string RolNombre);
}
