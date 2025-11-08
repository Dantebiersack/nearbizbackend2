namespace nearbizbackend.DTOs
{
    public record Paged<T>(IEnumerable<T> Items, int Total);
}