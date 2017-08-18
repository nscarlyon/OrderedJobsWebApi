using OrderedJobs.Data;
using OrderedJobs.Domain;
using System.Threading.Tasks;

namespace OrderedJobs.Web
{
    public class Result
    {
        public string testCase;
        public string result;

        public Result(string permutation, string url, IHttpClient httpClient)
        {
            testCase = permutation;
            var unverifiedHttpResponse = GetResult(permutation, url, httpClient).Result;
            result = VerifiedJobs.GetVerifiedResult(permutation, unverifiedHttpResponse);
        }

        public async Task<string> GetResult(string permutation, string url, IHttpClient httpClient)
        {
            var orderedJobsResponse = await httpClient.GetAsync(url + '/' + permutation);
            var unverifiedHttpResponse = await orderedJobsResponse.Content.ReadAsStringAsync();
            return unverifiedHttpResponse;
        }
    }
}