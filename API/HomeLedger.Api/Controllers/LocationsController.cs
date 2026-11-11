using AutoMapper;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using Microsoft.AspNetCore.Mvc;
namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController(
    ILocationUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocationDto>>> GetAll(CancellationToken ct)
    {
        var locations = await useCase.GetAllAsync(ct);
        return Ok(mapper.Map<IReadOnlyList<LocationDto>>(locations));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocationDto>> Get(int id, CancellationToken ct)
    {
        var location = await useCase.GetAsync(id, ct);
        if (location is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<LocationDto>(location));
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create(LocationDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Location>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<LocationDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LocationDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Location>(dto with { Id = id });
        var updated = await useCase.UpdateAsync(id, model, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await useCase.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
