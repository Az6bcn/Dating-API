using DatingAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Dating_API.Controllers
{
    //[Authorize] // ==> only authenticated user! (user with valid jwt)
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly DataDbContext _dataDbContext;

        // DI to get configured DbContext in services
        public ValuesController(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }

        // GET api/values
        [HttpGet]
        // IActionResult => return type use to return http results/responses like ok, badrequest, etc
        public async Task<IActionResult> GetValues() 
        {
            // use context to go to DB and fetch data
            var values = await _dataDbContext.Values.ToListAsync();

            return  Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var value = await _dataDbContext.Values.FirstOrDefaultAsync(x => x.Id == id);

            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

