using no.miles.at.Backend.Infrastructure;

namespace no.miles.at.Backend.Worker
{
    static class Program
    {
        

        static void Main()
        {
            var logger = new ConsoleLogger();

            var worker = new WorkerProcess(logger);

            worker.Start();

            System.Console.WriteLine("Press enter to finish");
            System.Console.ReadLine();
            System.Console.WriteLine("Stopping.......");

            worker.Stop();
            logger.Info("Stopped");
        }
    }
}
