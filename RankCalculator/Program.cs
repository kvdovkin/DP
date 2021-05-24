using System;
using Valuator;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            IStorage getRank = new RedisStorage();

            var calculateRank = new RankCalculator(getRank);
            calculateRank.Run();
        }
    }
}
