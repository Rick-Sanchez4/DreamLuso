using DreamLuso.Application.CQ.Properties.Commands.CreateProperty;
using DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly ISender _sender;

    public PropertyController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProperty([FromForm] CreatePropertyCommand command)
    {
        var result = await _sender.Send(command);
        
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateProperty(Guid id, [FromForm] UpdatePropertyCommand command)
    {
        // Validate that route id matches command id
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID mismatch", routeId = id, commandId = command.Id });
        }
        
        var result = await _sender.Send(command);
        
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}

