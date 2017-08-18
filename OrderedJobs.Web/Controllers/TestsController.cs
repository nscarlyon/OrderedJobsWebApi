using Microsoft.AspNetCore.Mvc;
using OrderedJobs.Data;

namespace OrderedJobs.Web.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class TestsController : Controller
    {
        IMongoDatabase mongoDatabase;
        TestCasesHelper testCasesHelper;

        public TestsController(IMongoDatabase mongoDatabase, TestCasesHelper testCasesHelper)
        {
            this.mongoDatabase = mongoDatabase;
            this.testCasesHelper = testCasesHelper;
        }

        [HttpGet]
        public PassFailResults Get([FromQuery]string url)
        {
            return testCasesHelper.GetPassFailResults(url);
        }

        [HttpPost]
        public void Post([FromBody] Data.TestCase value)
        {
            mongoDatabase.InsertTestCase(value);
        }

        [HttpDelete]
        public void Delete()
        {
            mongoDatabase.DeleteTestCases();
        }
    }
}
