namespace nearbizbackend.DTOs
{
    public record CategoriaReadDto(int IdCategoria, string NombreCategoria, bool Estado);
    public record CategoriaCreateDto(string NombreCategoria);
    public record CategoriaUpdateDto(string NombreCategoria);
}