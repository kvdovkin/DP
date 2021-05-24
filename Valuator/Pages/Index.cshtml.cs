using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            CancellationTokenSource cts = new CancellationTokenSource();
            await TaskCalculatingRank(id);

            return Redirect($"summary?id={id}");
        }

        private async Task TaskCalculatingRank(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection connection = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                connection.Publish("valuator.processing.rank", data);
                await Task.Delay(1000);

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
    }
}
