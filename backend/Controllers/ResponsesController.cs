using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponsesController(IFormResponseRepository repository) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(FormResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<FormResponse> Create([FromBody] FormResponse response)
    {
        var savedResponse = repository.Save(response);
        return Created($"/api/responses/{savedResponse.Id}", savedResponse);
    }
}