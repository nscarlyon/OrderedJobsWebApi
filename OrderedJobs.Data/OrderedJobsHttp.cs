using System.Threading.Tasks;
using System.Net.Http;

namespace OrderedJobs.Data
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }

    public class OrderedJobsHttpClient : IHttpClient
    {
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            var httpClient = new HttpClient();
            return httpClient.GetAsync(url);
        }
    }
}