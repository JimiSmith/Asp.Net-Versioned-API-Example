using Jimismith.Versioning.Example.Models;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Jimismith.Versioning.Example.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        [Route]
        [HttpGet]
        [MapToApiVersion("1.0")]
        // the query param is named with the version here otherwise only one of these methods will show up in swagger
        public IEnumerable<string> GetV1(string queryV1)
        {
            return new int[] { 1, 2, 3 }.Select(i => $"{queryV1}-{i}");
        }

        [Route]
        [HttpGet]
        [MapToApiVersion("1.1")]
        public V1_1ViewModel GetV1_1(string queryV1_1)
        {
            return new V1_1ViewModel
            {
                Result = new int[] { 1, 2, 3 }.Select(i => $"{queryV1_1}-{i}")
            };
        }

        [Route("all")]
        [HttpGet]
        public IEnumerable<string> GetAll(string query)
        {
            return new int[] { 1, 2, 3 }.Select(i => $"{query}-all-{i}");
        }
    }
}
