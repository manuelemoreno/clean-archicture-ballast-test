using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.WebAPI.Constants;
using AwesomeFruits.WebAPI.Exceptions;
using AwesomeFruits.WebAPI.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeFruits.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FruitsController : ControllerBase
{
    private readonly IFruitService _fruitService;

    public FruitsController(IFruitService fruitService)
    {
        _fruitService = fruitService;
    }

    // GET: api/Fruits
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var result = await _fruitService.FindAllFruitsAsync();

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, ResponseConstants.InternalError);
        }
    }

    // GET: api/Fruits/{id}
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var result = await _fruitService.FindFruitByIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is EntityNotFoundException or ValidationErrorsException) throw;
            return StatusCode(500, ResponseConstants.InternalError);
        }
    }

    // POST: api/Fruits
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SaveFruitDto request)
    {
        try
        {
            ValidateSaveFruitDtoRequest(request);

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userNameId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            request.CreatedByUserId = userNameId;

            return Created("", await _fruitService.SaveFruitAsync(request));
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case FruitAlreadyExistsException:
                    return BadRequest(ex.Message);
                case ValidationErrorsException:
                    throw;
                default:
                    return StatusCode(500, ResponseConstants.InternalError);
            }
        }
    }

    // PUT: api/Fruits
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateFruitDto request)
    {
        try
        {
            ValidateUpdateFruitDtoRequest(request);

            await _fruitService.UpdateFruitAsync(request);

            return NoContent();
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case FruitAlreadyExistsException:
                    return BadRequest(ex.Message);
                case ValidationErrorsException:
                    throw;
                default:
                    return StatusCode(500, ResponseConstants.InternalError);
            }
        }
    }

    // DELETE: api/Fruits
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _fruitService.DeleteFruitAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex is EntityNotFoundException) throw;
            return StatusCode(500, ResponseConstants.InternalError);
        }
    }

    private static void ValidateSaveFruitDtoRequest(SaveFruitDto request)
    {
        var errors = new SaveFruitDtoValidator().ValidateSaveFruitDto(request);

        if (errors.Count > 0) throw new ValidationErrorsException(errors);
    }

    private static void ValidateUpdateFruitDtoRequest(UpdateFruitDto request)
    {
        var errors = new UpdateFruitDtoValidator().ValidateUpdateFruitDto(request);

        if (errors.Count > 0) throw new ValidationErrorsException(errors);
    }
}