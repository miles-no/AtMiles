using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.Worker.Properties;

namespace no.miles.at.Backend.Worker
{
    static class Program
    {
        

        static void Main()
        {
            var logger = new ConsoleLogger();
            var configFilename = Settings.Default.ConfigFile;

            var worker = new WorkerProcess(logger, configFilename);

            worker.Start();

            System.Console.WriteLine("Press enter to finish");
            System.Console.ReadLine();
            System.Console.WriteLine("Stopping.......");

            worker.Stop();
            logger.Info("Stopped");
        }
    }
}
