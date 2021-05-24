using System;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Logger!");

            var eventsLogger = new EventsLogger();
            eventsLogger.Run();
        }
    }
}
