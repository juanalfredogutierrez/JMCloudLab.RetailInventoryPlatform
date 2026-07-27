namespace TransaccionService.Application.DTOs
{
    public class ApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public string TraceId { get; set; } = string.Empty;
    }
}
