using Jkh.AspireDashboardHosting;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting");

            CancellationTokenSource stopper = new();
            AspireDashboardHost host = new();
            //NOTE: the Dashboard listens on "Any" (0.0.0.0) so remote machines could reach us
            var aspireDashboardTask = host.StartAspireDashboard("http://0.0.0.0:9999", 4317, stopper.Token);

            Console.WriteLine("Press ENTER to quit!");
            Console.ReadLine();
            Console.WriteLine("Stopping");

            stopper.Cancel();
            await aspireDashboardTask;

            Console.WriteLine("BYE!");
        }
    }
}
