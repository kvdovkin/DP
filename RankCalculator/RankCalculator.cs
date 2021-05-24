using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            _asyncSubscription = _connection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args)
                =>
            {
                string key = Encoding.UTF8.GetString(args.Message.Data);

                string textKey = Constants.TextKey + key;
                string text = storage.Load(textKey);

                string rankKey = Constants.RankKey + key;
                string rank = GetRank(text).ToString();
                storage.Store(rankKey, rank);

                Console.WriteLine("Existed: {0} from subject {1}", key, args.Message.Subject);
            });
        }
        private static double GetRank(string text)
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





    }
}
