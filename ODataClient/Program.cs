
using Simple.OData.Client;

internal class Program
{
    static int _semaphoreCount = 0;
    static int _taskNumber = 0;
    static int _numberOfRequests = 0;
    private static SemaphoreSlim _semaphore;
    static ODataClientSettings _settings = new ODataClientSettings()
    {
        BaseUri = new Uri("http://localhost:81/odata/"),
        IgnoreResourceNotFoundException = true,
        IgnoreUnmappedProperties = true,
        //RequestTimeout = TimeSpan.FromMilliseconds(2000),
        PayloadFormat = ODataPayloadFormat.Json,
        RenewHttpConnection = true,
    };
    static IODataClient _client;


    static async Task Main(string[] args)
    {
        int waitTime = 0;
        //_settings.AfterResponse += delegate (HttpResponseMessage message) { Console.WriteLine(message.StatusCode); };
        _client = new ODataClient(_settings);
        if (args.Length > 0)
        {
            waitTime = Int32.Parse(args[0]);
        }
        MakeThreads(waitTime);

    }
    /*This method uses RetryHelper with RetryOnExceptionAsync to make sure the requests makes it through.
     * If the response from the server is an error/exception, this makes sure that the request waits and tries 
     * again five times using exponential backoff.
     * Each retry waits longer before reattempting the request.
     * */
    //static async Task GetMockODataAsync()
    //{

    //    await _semaphore.WaitAsync();
    //    Console.WriteLine("Task {0} enters the _semaphore.", Task.CurrentId);
    //    try
    //    {
    //        var maxRetryAttempts = 5;
    //        string json = "{Here you should have a valid JSON object to POST}";

    //        await RetryHelper.RetryOnExceptionAsync<HttpRequestException>(maxRetryAttempts, async () =>
    //        {
    //            //Thread.Sleep(500);
    //            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    //            var packages = await client.For<Odatamock>().FindEntriesAsync();

    //            Interlocked.Increment(ref _requestNumber);
    //            Interlocked.Increment(ref _taskNumber);
    //            foreach (var package in packages)
    //            {
    //                Console.WriteLine($"Request number: {_requestNumber}   Name: {package.Name}");
    //            }
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Exception: " + ex.Message);
    //    }
    //    finally
    //    {
    //        Interlocked.Decrement(ref _taskNumber);
    //        Console.WriteLine($"Number of running requests after finishing one: {_taskNumber}");
    //        _semaphore.Release();
    //    }
    //    //Console.WriteLine("Task {0} releases the _semaphore; previous count: {1}.",
    //    //                          _taskNumber, _semaphoreCount);
    //}


    static void MakeThreads(int waitTime)
    {
        int maxSemaphores = 52;
        int numTasks = 500;
        int numTasksWithWaiting = 100;
        _semaphore = new SemaphoreSlim(0, maxSemaphores);

        //_semaphore.CurrentCount returns the remaining number of threads that can enter the semaphore
        Console.WriteLine("{0} tasks can enter the _semaphore.",
                          _semaphore.CurrentCount);

        List<Task> tasks = new List<Task>();
        int f = 0;
        for (int v = 0; v < numTasksWithWaiting; v++)
        {
            var task = Task.Run(() => GetMockODataAsync(waitTime));
            tasks.Add(task);
            f++;
        }
        for (int i = 0; i < numTasks; i++)
        {
            var task = Task.Run(() => GetMockODataAsync(0));
            tasks.Add(task);
            f++;
        }



        //Thread.Sleep(500);
        Console.WriteLine($"Number of threads: {f}");

        _semaphore.Release(maxSemaphores);

        Console.WriteLine("{0} tasks can enter the _semaphore.",
                          _semaphore.CurrentCount);

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine("Main thread exits.");
    }
    static async Task GetMockODataAsync(int waitTime)
    {

        await _semaphore.WaitAsync();
        Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);
        try
        {

            Interlocked.Increment(ref _numberOfRequests);
            var packages = await _client.For<Odatamock>().Key(waitTime).FindEntriesAsync();
            //Thread.Sleep(500);

            Interlocked.Increment(ref _taskNumber);
            foreach (var package in packages)
            {
                Console.WriteLine($"Request number: {_taskNumber}   Name: {package.Name}");
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }

        finally
        {
            _semaphoreCount = _semaphore.Release();
            Interlocked.Decrement(ref _numberOfRequests);
            Console.WriteLine($"Number of requests running: {_numberOfRequests}");
        }
        Console.WriteLine("Task {0} releases the semaphore; previous count: {1}.",
                                  _taskNumber, _semaphoreCount);
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
