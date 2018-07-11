using System;
using Microsoft.AspNetCore.Mvc;

namespace ClassLibrary1
{
    [Route("api/{controller}")]
    public class FooController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string[]> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
