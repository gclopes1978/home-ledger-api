using AutoMapper;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using Microsoft.AspNetCore.Mvc;
namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/groupings")]
public sealed class GroupingsController(
    IGroupingUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GroupingDto>>> GetAll(CancellationToken ct)
    {
        var groupings = await useCase.GetAllAsync(ct);
        return Ok(mapper.Map<IReadOnlyList<GroupingDto>>(groupings));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GroupingDto>> Get(int id, CancellationToken ct)
    {
        var grouping = await useCase.GetAsync(id, ct);
        if (grouping is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<GroupingDto>(grouping));
    }

    [HttpPost]
    public async Task<ActionResult<GroupingDto>> Create(GroupingDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Grouping>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<GroupingDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, GroupingDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Grouping>(dto with { Id = id });
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
