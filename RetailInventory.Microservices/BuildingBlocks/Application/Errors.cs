namespace BuildingBlocks.Application
{
    public static class Errors
    {
        public static Error Validation(string message)
            => new("Validation", message);

        public static Error NotFound(string message)
            => new("NotFound", message);

        public static Error Unauthorized(string message)
            => new("Unauthorized", message);

        public static Error Conflict(string message)
            => new("Conflict", message);
    }
}
