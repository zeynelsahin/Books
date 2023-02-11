using Microsoft.AspNetCore.Mvc;

namespace BookCovers.API.Controllers;

[Route("api/bookCovers")]
[ApiController]
public class BookCoversController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookCover(string id, bool returnFault = false)
    {
        if (returnFault)
        {
            await Task.Delay(100);
            return new StatusCodeResult(500);
        }

        var random = new Random();
        var fakeCoverBytes = random.Next(4561161, 11314955);
        var fakeCover = new byte[fakeCoverBytes];
        random.NextBytes(fakeCover);
        return Ok(new
        {
            Id = id,
            Content = fakeCover
        });
    }
}