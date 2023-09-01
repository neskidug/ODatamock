public class Program
{
    static HttpClient client;
    private static void Main(string[] args)
    {
        MakeThreads();


    }
    static async Task GetMockODataAsync()
    {


        client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:5000/odata/Odatamock");
            Console.WriteLine($"{response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                string GetMockOData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"{response.StatusCode}{GetMockOData}");
            }
            else
            {
                Console.WriteLine("Error");
            }

        }
        catch (AggregateException aex)
        {
            Console.WriteLine("Handle Remaining Exceptions");
            foreach (Exception ex in aex.InnerExceptions)
            {
                Console.WriteLine("Begynder her");
                Console.WriteLine("{0}: {1}", ex.GetType().Name, ex.Message);
            }
        }
    }

    static void MakeThreads()
    {
        int numThreads = 100; // Change this to the number of threads you want to create


        Thread[] threads = new Thread[numThreads];
        List<Task> tasks = new List<Task>();
        int f = 0;
        for (int i = 0; i < numThreads; i++)
        {
            //threads[i] = new Thread(GetMockODataAsync);
            var task = GetMockODataAsync();
            tasks.Add(task);
            //threads[i].Start();

            f++;
        }
        Console.WriteLine(f);

        Task.WaitAll(tasks.ToArray());
    }
}



