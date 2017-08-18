using OrderedJobs.Data;
using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Web
{
    public class PassFailTestCase
    {
        public string testCase;
        public string Result;
        public IEnumerable<Result> Results;

        public PassFailTestCase(List<string> permutations, string url, IHttpClient httpClient)
        {
            testCase = permutations[0];
            Results = GetPermutationResults(permutations, url, httpClient);
            Result = Results.All(x => x.result == "Pass") ? "Pass" : "Fail";
        }

        public IEnumerable<Result> GetPermutationResults(List<string> permutations, string url, IHttpClient httpClient)
        {
            var permutationResults = permutations.Select(permutation =>
            {
              return new Result(permutation, url, httpClient);
            });

            return permutationResults;
        }
    }
}