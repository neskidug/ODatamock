using Simple.OData.Client;

public class Program
{
    static int _taskNumber = 0;
    static ODataClientSettings _settings = new ODataClientSettings()
    {
        BaseUri = new Uri("http://localhost:81/odata/"),
        IgnoreResourceNotFoundException = true,
        IgnoreUnmappedProperties = true,
        //RequestTimeout = TimeSpan.FromMilliseconds(2000),
        PayloadFormat = ODataPayloadFormat.Json,
        RenewHttpConnection = true,
    };
    static IODataClient client;


    static async Task Main(string[] args)
    {
        int waitTime = 0;
        //_settings.AfterResponse += delegate (HttpResponseMessage message) { Console.WriteLine(message.StatusCode); };
        client = new ODataClient(_settings);
        if (args.Length > 0)
        {
            waitTime = Int32.Parse(args[0]);
        }
        MakeThreads(waitTime);

    }

    static void MakeThreads(int waitTime)
    {
        int numTasks = 500;
        int numTasksWithWaiting = 100;

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

        Console.WriteLine($"Number of threads: {f}");

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine("Main thread exits.");
    }
    static async Task GetMockODataAsync(int waitTime)
    {
        try
        {
            var packages = await client.For<Odatamock>().Key(waitTime).FindEntriesAsync();
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
}



