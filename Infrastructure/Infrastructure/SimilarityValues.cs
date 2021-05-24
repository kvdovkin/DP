using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    [Serializable]
    public struct SimilarityValues
    {
        public string Id { get; set; }
        public double Similarity { get; set; }

        public SimilarityValues(string id, double similarity)
        {
            this.Id = id;
            this.Similarity = similarity;
        }
    }
}
