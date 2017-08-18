using OrderedJobs.Data;
using OrderedJobs.Domain;
using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Web
{
    public class PassFailResults
    {
        public string Result;
        public IEnumerable<PassFailTestCase> Results;

        public PassFailResults(IEnumerable<TestCase> testCasesFromDatabase, string url, IHttpClient httpClient)
        {
            Results = GetPassFailTestCases(testCasesFromDatabase, url, httpClient);
            Result = (Results.All((testCase) => testCase.Result == "Pass")) ? "Pass" : "Fail";
        }

        public IEnumerable<PassFailTestCase> GetPassFailTestCases(IEnumerable<TestCase> testCasesFromDatabase, string url, IHttpClient httpClient)
        {
            var passFailTestCases = testCasesFromDatabase.Select(testCase =>
            {
                var permutations = Permutations.GetPermutations(testCase);
                return new PassFailTestCase(permutations, url, httpClient);
            });

            return passFailTestCases;
        }
    }
}