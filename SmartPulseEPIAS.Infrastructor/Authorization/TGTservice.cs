using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;


namespace SmartPulseEPIAS.Infrastrucator.Authorization
{
    public class TGTservice
    {
        
            private readonly HttpClient _client;
            private readonly IMemoryCache _cache;
            private const string CACHE_KEY = "TGT_TOKEN";
            private readonly IConfiguration _configuration;

            public TGTservice(HttpClient client, IConfiguration configuration)
            {
                _client = client;

            _cache = new MemoryCache(new MemoryCacheOptions());

            _configuration = configuration;

            }


            public async Task<string> CacheToken()
            {

                if (_cache.TryGetValue(CACHE_KEY, out string token))
                    return token;

                var response = await Login();
                _cache.Set(CACHE_KEY, response, TimeSpan.FromHours(2));
                return response;


            }

            public async Task<string> Login()
            {
                var mailcontent = _configuration["ConnectionStrings:mail"];
                var passwordcontent = _configuration["ConnectionStrings:password"];

                var content = new FormUrlEncodedContent(new[]
                {

                new KeyValuePair <string,string>("username", mailcontent),
                 new KeyValuePair <string,string>("password", passwordcontent)

            });
                var request = new HttpRequestMessage(HttpMethod.Post, "https://giris.epias.com.tr/cas/v1/tickets");
                request.Content = content;
                request.Headers.Add("Accept", "text/plain");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var tgt = await response.Content.ReadAsStringAsync();
                return tgt;


            }


        }
    }


