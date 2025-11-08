namespace nearbizbackend.DTOs
{
    public record CitaReadDto(int IdCita, int IdCliente, int IdTecnico, int IdServicio, DateOnly FechaCita, TimeOnly HoraInicio, TimeOnly HoraFin, string Estado, string? MotivoCancelacion, DateTime FechaCreacion, DateTime FechaActualizacion);
    public record CitaCreateDto(int IdCliente, int IdTecnico, int IdServicio, DateOnly FechaCita, TimeOnly HoraInicio, TimeOnly HoraFin);
    public record CitaUpdateDto(int IdCliente, int IdTecnico, int IdServicio, DateOnly FechaCita, TimeOnly HoraInicio, TimeOnly HoraFin, string Estado);
    public record CitaCancelDto(string MotivoCancelacion);
}
