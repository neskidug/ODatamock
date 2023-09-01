namespace OData_mock.MiddleWare
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggerMiddleware> _logger;
        private static int _currentConnections = 0;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            Interlocked.Increment(ref _currentConnections);
            _logger.LogInformation($"Current Connections: {_currentConnections}");

            try
            {
                await _next(context);
            }
            finally
            {
                Interlocked.Decrement(ref _currentConnections);
            }
        }
    }
}
