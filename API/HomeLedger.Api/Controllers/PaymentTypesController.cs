using AutoMapper;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;
using Microsoft.AspNetCore.Mvc;
namespace HomeLedger.Api.Controllers;

[ApiController]
[Route("api/paymenttypes")]
public sealed class PaymentTypesController(
    IPaymentTypeUseCase useCase,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentTypeDto>>> GetAll(CancellationToken ct)
    {
        var paymentTypes = await useCase.GetAllAsync(ct);
        return Ok(mapper.Map<IReadOnlyList<PaymentTypeDto>>(paymentTypes));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentTypeDto>> Get(int id, CancellationToken ct)
    {
        var paymentType = await useCase.GetAsync(id, ct);
        if (paymentType is null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<PaymentTypeDto>(paymentType));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentTypeDto>> Create(PaymentTypeDto dto, CancellationToken ct)
    {
        var model = mapper.Map<PaymentType>(dto);
        var created = await useCase.InsertAsync(model, ct);
        var result = mapper.Map<PaymentTypeDto>(created);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PaymentTypeDto dto, CancellationToken ct)
    {
        var model = mapper.Map<PaymentType>(dto with { Id = id });
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
