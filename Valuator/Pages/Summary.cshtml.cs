using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Infrastructure;
using Infrastructure.Storage;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            string getSKey = _storage.GetSKey(id); //получаем значения ключа 
            _logger.LogDebug($"LOOKUP: {id}, {_storage.GetSKey(id)}.");

            //TODO: проинициализировать свойства Rank и Similarity сохранёнными в БД значениями

            string similarityKey = Constants.SimilarityKey + id;
            Similarity = Convert.ToDouble(_storage.Load(getSKey, similarityKey));


            string rankKey = Constants.RankKey + id;
            int count = 0;
            while (count < 100)
            {
                Thread.Sleep(100);
                if (_storage.CheckingKey(getSKey,rankKey))
                {
                    Rank = Convert.ToDouble(_storage.Load(getSKey, rankKey));
                }

                break;
            }
        }
    }
      
}

