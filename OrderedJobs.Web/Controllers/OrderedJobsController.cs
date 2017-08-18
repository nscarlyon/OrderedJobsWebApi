using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using OrderedJobs.Data;

namespace OrderedJobs.Web.Controllers
{
    [Route("[controller]")]
    public class OrderedJobsController : Controller
    {
        IMongoDatabase mongoDatabase;

        public OrderedJobsController(IMongoDatabase mongoDatabase)
        {
            this.mongoDatabase = mongoDatabase;
        }

        [HttpGet]
        public IEnumerable<TestCase> Get()
        {
            return mongoDatabase.GetAllJobs();
        }

        [HttpGet("{dependencies}")]
        public string Get(string dependencies)
        {
            return Domain.OrderedJobs.GetOrderedJobs(dependencies);
        }
    }
}
