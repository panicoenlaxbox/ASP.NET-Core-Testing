using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace XUnitTestProject1
{
    public static class RequestBuilderExtensions
    {
        public static async Task<T> GetTo<T>(this RequestBuilder builder)
        {
            var response = await builder.GetAsync();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}