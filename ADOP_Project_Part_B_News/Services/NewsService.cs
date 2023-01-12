//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Threading.Tasks;
using System.Xml.Serialization;

using ADOP_Project_Part_B_News.Models;
using ADOP_Project_Part_B_News.ModelsSampleData;

namespace ADOP_Project_Part_B_News.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
        //Your API key
        readonly string apiKey = "6b2990a1f70040498e70f10e83807d53";
        //readonly string apiKey = "";

        FileNews fileNews = new FileNews();

        public event EventHandler<string> NewsDataAvailable;
        protected virtual void OnNewsDataAvailable(string message)
        {
            NewsDataAvailable?.Invoke(this, message);
        }

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
            News news = null;
            var key = fileNews.ConvertToFileName(category, DateTime.Now.ToString("yyyy-MM-dd HH-mm"));

            if (!File.Exists(key))
            {
#if UseNewsApiSample
                NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

#else
                //https://newsapi.org/docs/endpoints/top-headlines
                var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}";

                // make the http request
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                //Convert Json to Object
                NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();
#endif
                news = new News()
                {
                    Category = category,
                    Articles = nd.Articles.Select(ndi => new NewsItem()
                    {
                        DateTime = ndi.PublishedAt,
                        Title = ndi.Title,
                        Url = ndi.Url,
                        UrlToImage = ndi.UrlToImage
                    }).ToList()
                };
                return news;
            }
            news = await FileNews.DeserializeCacheAsync(news, key);
            OnNewsDataAvailable($"XML Cached News for category {category} is available");
            return news;
        }
    }

    public class FileNews
    {
        public string ConvertToFileName(NewsCategory n, string dt)
        {
            return Fname($"Cached_News_for_{n}_{dt}.xml");
        }
        static string Fname(string name)
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documentPath = Path.Combine(documentPath, "NewsService");
            if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
            return Path.Combine(documentPath, name);
        }
        static public Task SerializeCacheAsync(News news, string name)
        {
            return Task.Run(() => SerializeCache(news, name));
        }

        static public void SerializeCache(News news, string name)
        {
            object _lock = new object();
            lock (_lock)
            {
                XmlSerializer xs = new XmlSerializer(typeof(News));
                using (Stream s = File.Create(name))
                {
                    xs.Serialize(s, news);
                }
            }
        }
        static public Task<News> DeserializeCacheAsync(News news, string name)
        {
            return Task.Run(() => DeserializeCache(news, name));
        }

        static public News DeserializeCache(News news, string name)
        {
            object _lock = new object();
            lock (_lock)
            {
                XmlSerializer xs = new XmlSerializer(typeof(News));
                using (Stream s = File.OpenRead(name))
                {
                    News returnNews = (News)xs.Deserialize(s);
                    return returnNews;
                }
            }
        }
    }
}
