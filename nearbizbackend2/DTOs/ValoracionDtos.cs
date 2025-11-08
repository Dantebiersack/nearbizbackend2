namespace nearbizbackend.DTOs
{
    public record ValoracionReadDto(int IdValoracion, int IdCita, int IdCliente, int IdNegocio, int Calificacion, string? Comentario, DateTime Fecha, bool Estado);
    public record ValoracionCreateDto(int IdCita, int IdCliente, int IdNegocio, int Calificacion, string? Comentario);
    public record ValoracionUpdateDto(int Calificacion, string? Comentario);
}
