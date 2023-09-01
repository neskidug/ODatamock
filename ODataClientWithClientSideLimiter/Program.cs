
using Simple.OData.Client;
using System.Threading.RateLimiting;

internal class Program
{
    static int _taskNumber = 0;
    static int _requestNumber = 0;
    static ODataClientSettings settings = new ODataClientSettings()
    {
        BaseUri = new Uri("http://localhost:81/odata/"),
        IgnoreResourceNotFoundException = true,
        IgnoreUnmappedProperties = true,
        //RequestTimeout = TimeSpan.FromMilliseconds(2000),
        PayloadFormat = ODataPayloadFormat.Json,
        RenewHttpConnection = true,
    };
    static IODataClient _client;
    static RateLimiter _limiter;

    static async Task Main(string[] args)
    {
        int waitTime = 0;
        _limiter = new ConcurrencyLimiter(
            new ConcurrencyLimiterOptions() { PermitLimit = 50, QueueProcessingOrder = QueueProcessingOrder.OldestFirst, QueueLimit = 50 });
        settings.AfterResponse += delegate (HttpResponseMessage message) { Console.WriteLine(message.StatusCode); };
        _client = new ODataClient(settings);
        if (args.Length > 0)
        {
            waitTime = Int32.Parse(args[0]);
        }
        MakeThreads(waitTime);
    }

    static async Task GetMockODataAsync(int waitTime)
    {

        try
        {
            var packages = await _client.For<Odatamock>().Key(waitTime).FindEntriesAsync();
            Interlocked.Increment(ref _requestNumber);
            foreach (var package in packages)
            {
                Interlocked.Increment(ref _taskNumber);
                Console.WriteLine($"Request number: {_requestNumber}   Name: {package.Name}   WaitTime: {waitTime}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
        finally
        {

        }
    }



    static void MakeThreads(int waitTime)
    {
        int numThreads = 500;

        Task[] threads = new Task[numThreads];
        List<Task> tasks = new List<Task>();
        int f = 0;
        for (int i = 0; i < numThreads; i++)
        {
            using RateLimitLease lease = _limiter.AttemptAcquire(permitCount: 1);
            if (lease.IsAcquired)
            {
                Console.WriteLine("lease acquired");
                var task = Task.Run(() => GetMockODataAsync(0));
                Thread.Sleep(100);
                tasks.Add(task);
                Console.WriteLine("Lease released");
            }
            f++;
        }
        var t = Task.Run(() => GetMockODataAsync(waitTime));
        tasks.Add(t);
        Console.WriteLine($"Number of threads: {f}");

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine("Main thread exits.");
    }
}

public class Odatamock
{

    public int Id { get; set; }
    public string? Name { get; set; }
    public Odatamock(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
    public Odatamock() { }

}
