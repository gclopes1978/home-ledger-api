using AutoMapper;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using Microsoft.AspNetCore.Mvc;
namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/sources")]
public sealed class SourcesController(
    ISourceUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SourceDto>>> GetAll(CancellationToken ct)
    {
        var sources = await useCase.GetAllAsync(ct);
        return Ok(mapper.Map<IReadOnlyList<SourceDto>>(sources));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SourceDto>> Get(int id, CancellationToken ct)
    {
        var source = await useCase.GetAsync(id, ct);
        if (source is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<SourceDto>(source));
    }

    [HttpPost]
    public async Task<ActionResult<SourceDto>> Create(SourceDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Source>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<SourceDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SourceDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Source>(dto with { Id = id });
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
