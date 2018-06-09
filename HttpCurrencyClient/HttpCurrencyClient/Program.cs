using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpCurrencyClient
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void SendCurrency(Currency currency)
        {
            Console.WriteLine($"Currency: {currency.Symbol}\tPrice: " +
                $"{currency.Price}");

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "192.168.1.30:9092" }
            };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                var dr = producer.ProduceAsync(currency.Symbol + "-topic", null, $"Currency: {currency.Symbol}\tPrice: " +
                    $"{currency.Price}").Result;
                Console.WriteLine($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            }
        }

        static async Task<List<Currency>> GetCurrencyAsync(string path, string apiKey, string currenciesNames)
        {
            List<Currency> currencies = null;
            HttpResponseMessage response = await client.GetAsync("quotes/?pairs=" + currenciesNames + "&api_key="+ apiKey);
            if (response.IsSuccessStatusCode)
            {
                currencies = await response.Content.ReadAsAsync<List<Currency>>();
            }
            return currencies;
        }

        static void Main()
        {
            client.BaseAddress = new Uri("https://forex.1forge.com/1.0.3/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            while (true)
            {
                Task.Run(() => RunAsync("ETHUSD"));
                Task.Run(() => RunAsync("BTCUSD"));
                Task.Run(() => RunAsync("LTCUSD"));
                Task.Run(() => RunAsync("XRPUSD"));
                Task.Run(() => RunAsync("DSHUSD"));
                Task.Run(() => RunAsync("BCHUSD"));
                System.Threading.Thread.Sleep(5000);
            }
        }

        static async Task RunAsync(string currencyName)
        {
            try
            {
                // Get the currency
                List<Currency> currencies = await GetCurrencyAsync(client.BaseAddress.ToString(), 
                    "CmRPGdFegd5R2P8IKuQzAEei3d3xmn0m", currencyName);
                currencies.ForEach(currency => SendCurrency(currency));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
