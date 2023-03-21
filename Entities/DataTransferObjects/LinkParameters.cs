using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;

namespace Entities.DataTransferObjects;
public record LinkParameters(BookParameters BookParameters, HttpContext HttpContext);