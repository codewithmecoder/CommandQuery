using CommandQuery;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ICommandQuery commandQuery) : ControllerBase
    {
        [HttpGet("test")]
        public async Task<IActionResult> Get()
        {
            await commandQuery.Send(new RequestTest
            {
                Id = Guid.NewGuid().ToString()
            });
            var a = await commandQuery.Send(new RequestTestReturn
            {
                Id = Guid.NewGuid().ToString()
            });
            return Ok(a);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var result = await commandQuery.Send(command);
            return Ok(result);
        }
    }
}
