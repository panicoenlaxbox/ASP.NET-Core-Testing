using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("mypolicy")]
    //[Authorize()]
    public class ValuesController : ControllerBase
    {
        private readonly FooContext _context;

        public ValuesController(FooContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return await _context.Foo.Select(f => f.Bar).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var foo = await _context.Foo.FindAsync(id);
            if (foo == null)
            {
                return NotFound(id);
            }
            return foo.Bar;
        }
    }
}
