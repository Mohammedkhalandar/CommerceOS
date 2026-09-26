using CommerceOS.Application.DTOs.Customers;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(
        CreateCustomerDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                customer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(
            cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
        {
            return NotFound(new
            {
                message = "Customer was not found."
            });
        }

        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _customerService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Customer was not found."
            });
        }

        return NoContent();
    }
}