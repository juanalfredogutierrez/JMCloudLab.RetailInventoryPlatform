namespace BuildingBlocks.Observability.Services
{
    public interface ICorrelationContext
    {
        string CorrelationId { get; }
        void SetCorrelationId(string correlationId);
    }
}
