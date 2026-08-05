using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Api.Controllers;

[ApiController]
[Route("hashes")]
public class HashesController : ControllerBase {

    private readonly IHashProcessingService _hashProcessingService;
    private readonly IHashQueryService _hashQueryService;

    public HashesController(IHashProcessingService hashProcessingService, IHashQueryService hashQueryService)
    {
        _hashProcessingService = hashProcessingService;
        _hashQueryService = hashQueryService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GenerateHashesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Generate([FromBody] GenerateHashesRequest? request, CancellationToken cancellationToken) {
        request ??= new GenerateHashesRequest();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _hashProcessingService.GenerateAsync(
            request,
            cancellationToken);

        return Ok(result);
    }                    

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HashCountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) {
        var response = await _hashQueryService.GetAllAsync(cancellationToken);

        return Ok(response);
    }
}