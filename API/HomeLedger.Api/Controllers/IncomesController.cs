using AutoMapper;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/incomes")]
public sealed class IncomesController(
    IIncomeUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IncomeDto>>> GetAll(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        CancellationToken ct)
    {
        var incomes = await useCase.GetAllAsync(startDate, endDate, ct);
        return Ok(mapper.Map<IReadOnlyList<IncomeDto>>(incomes));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncomeDto>> Get(int id, CancellationToken ct)
    {
        var income = await useCase.GetAsync(id, ct);
        if (income is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<IncomeDto>(income));
    }

    [HttpPost]
    public async Task<ActionResult<IncomeDto>> Create(IncomeDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Income>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<IncomeDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, IncomeDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Income>(dto with { Id = id });
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