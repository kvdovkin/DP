using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Storage;
using NATS.Client;
using Valuator;

namespace RankCalculator
{
    class RankCalculator
    {
        private readonly IConnection _connection;
        private readonly IAsyncSubscription _asyncSubscription;

        public RankCalculator(IStorage storage)
        {
            ConnectionFactory cf = new ConnectionFactory();
            _connection = cf.CreateConnection();

            _asyncSubscription = _connection.SubscribeAsync(Constants.RankCalculate, "rank_calculator", async (sender, args)
                =>
            {
                string key = Encoding.UTF8.GetString(args.Message.Data);

                string textKey = Constants.TextKey + key;
                string text = storage.Load(textKey);

                string rankKey = Constants.RankKey + key;
                double rank = GetRank(text);
                storage.Store(rankKey, rank.ToString());

                RankValues rankValues = new RankValues(key, rank);
                await GetRankInfoToLogger(rankValues);

                Console.WriteLine("Existed: {0} from subject {1}", key, args.Message.Subject);
            });
        }
        private double GetRank(string text)
        {
            var countRank = text.Where(x => !Char.IsLetter(x)).Count();

            return (double)countRank / text.Length;
        }
        public void Run()
        {
            Console.WriteLine("RankCalculator started");

            _asyncSubscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            _asyncSubscription.Unsubscribe();

            _connection.Drain();
            _connection.Close();
        }

        private async Task GetRankInfoToLogger(RankValues rankValues)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection connection = cf.CreateConnection())
            {
                var recording = JsonSerializer.Serialize(rankValues);

                connection.Publish(Constants.RankAssignment, Encoding.UTF8.GetBytes(recording));

                await Task.Delay(100);

                connection.Drain();
                connection.Close();
            }
        }





    }
}
