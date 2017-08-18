using OrderedJobs.Data;

namespace OrderedJobs.Web
{
    public class TestCasesHelper
    {
        IMongoDatabase mongoDatabase;
        IHttpClient httpClient;

        public TestCasesHelper(IMongoDatabase mongoDatabase, IHttpClient httpClient)
        {
            this.mongoDatabase = mongoDatabase;
            this.httpClient = httpClient;
        }

        public PassFailResults GetPassFailResults(string url)
        {
            var testCasesFromDatabase = mongoDatabase.GetAllJobs();
            return new PassFailResults(testCasesFromDatabase, url, httpClient);
        }
    }
}

