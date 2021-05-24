using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IStorage _storage;

        public IndexModel( IStorage storage)
        {
            _storage = storage;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string text)
        {
            string id = Guid.NewGuid().ToString();

            string similarityKey = Constants.SimilarityKey + id; //реорганизация принципа подсчета 
            double similarity = GetSimilarity(text);

            _storage.Store(similarityKey, similarity.ToString()); //преобразуем для корректного отображения

            string textKey = Constants.TextKey + id;
            _storage.Store(textKey, text);
            _storage.Load(textKey);

            await TaskCalculatingRank(id);

            SimilarityValues sendSimilarity = new SimilarityValues(id, similarity);

            await TaskSendingSimilarity(sendSimilarity);

            return Redirect($"summary?id={id}");
        }

        private async Task TaskCalculatingRank(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection connection = cf.CreateConnection())
            {
                byte[] rank = Encoding.UTF8.GetBytes(id);
                connection.Publish(Constants.RankCalculate, rank);
                await Task.Delay(100);

                connection.Drain();
                connection.Close();
            }
        }
        double GetSimilarity(string text)
        {
            var similarity = _storage.TextSignes("TEXT-", text);
            if (similarity)
            {
                return 1;
            }

            return 0;
        }

        private async Task TaskSendingSimilarity(SimilarityValues sendSimilarity)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection connection = cf.CreateConnection())
            {
                var similarity = JsonSerializer.Serialize(sendSimilarity);

                connection.Publish(Constants.SimilarityAssignment, Encoding.UTF8.GetBytes(similarity));
                await Task.Delay(100);

                connection.Drain();
                connection.Close();
            }
        }
    }
}
