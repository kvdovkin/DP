using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure;
using NATS.Client;

namespace EventsLogger
{
    public class EventsLogger
    {
        private readonly IAsyncSubscription _asyncSubscription;
        private readonly IConnection _connection;

        public EventsLogger()
        {
            ConnectionFactory cf = new ConnectionFactory();
            _connection = cf.CreateConnection();
            _asyncSubscription = _connection.SubscribeAsync(Constants.RankAssignment, (sender, args) =>
            {
                RankValues rank = JsonSerializer.Deserialize<RankValues>(args.Message.Data);

                Console.WriteLine($"Event: {args.Message.Subject}\n" +
                                  $"Id: {rank.Id}\n" +
                                  $"Rank: {rank.Rank}\n");
            });

            _asyncSubscription = _connection.SubscribeAsync(Constants.SimilarityAssignment, (sender, args) =>
            {
                SimilarityValues similarity = JsonSerializer.Deserialize<SimilarityValues>(args.Message.Data);

                Console.WriteLine($"Event: {args.Message.Subject}\n" +
                                  $"Id: {similarity.Id}\n" + 
                                  $"Similarity: {similarity.Similarity}\n");

            });
        }

        public void Run()
        {
            _asyncSubscription.Start();

            Console.WriteLine("Press \"Enter\" to exit.\n");
            Console.ReadLine();

            _asyncSubscription.Unsubscribe();

            _connection.Drain();
            _connection.Close();
        }
    }
}
