using AutoMapper;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using Microsoft.AspNetCore.Mvc;
namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(
    ICategoryUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var categories = await useCase.GetAllAsync(ct);
        return Ok(mapper.Map<IReadOnlyList<CategoryDto>>(categories));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Get(int id, CancellationToken ct)
    {
        var category = await useCase.GetAsync(id, ct);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<CategoryDto>(category));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Category>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<CategoryDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryDto dto, CancellationToken ct)
    {
        var model = mapper.Map<Category>(dto with { Id = id });
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
