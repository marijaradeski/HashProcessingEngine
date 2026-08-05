using HashProcessingEngine.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Application.Interfaces;
public interface IHashQueryService
{
    Task<HashCountResponse> GetAllAsync(CancellationToken cancellationToken);
}
