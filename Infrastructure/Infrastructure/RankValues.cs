using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    [Serializable]
    public struct RankValues
    {
        public string Id { get; set; }
        public double Rank { get; set; }

        public RankValues(string id, double rank)
        {
            this.Id = id;
            this.Rank = rank;
        }

    }

}
