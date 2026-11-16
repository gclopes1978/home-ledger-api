using AutoMapper;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public sealed class ExpensesController(
    IExpenseUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExpenseDto>>> GetAll(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        CancellationToken ct)
    {
        var expenses = await useCase.GetAllAsync(startDate, endDate, ct);
        return Ok(mapper.Map<IReadOnlyList<ExpenseDto>>(expenses));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseDto>> Get(int id, CancellationToken ct)
    {
        var expense = await useCase.GetAsync(id, ct);
        if (expense is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<ExpenseDto>(expense));
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create(ExpenseDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Expense>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<ExpenseDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ExpenseDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Expense>(dto with { Id = id });
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