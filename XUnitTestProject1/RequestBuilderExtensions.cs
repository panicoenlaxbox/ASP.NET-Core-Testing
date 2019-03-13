using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XUnitTestProject1
{
    public static class RequestBuilderExtensions
    {
        public static async Task<T> GetTo<T>(this HttpResponseMessage responseMessage)
        {
            var json = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}