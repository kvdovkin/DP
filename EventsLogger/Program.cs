using System;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Logger Started!");

            var eventsLogger = new EventsLogger();
            eventsLogger.Run();
        }
    }
}
