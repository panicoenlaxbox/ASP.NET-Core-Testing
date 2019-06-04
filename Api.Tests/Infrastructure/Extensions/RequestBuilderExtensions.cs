using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Api.Tests.Infrastructure.Extensions
{
    public static class RequestBuilderExtensions
    {
        public static async Task<T> GetAsyncTo<T>(this HttpResponseMessage responseMessage)
        {
            var json = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}