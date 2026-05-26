using Microsoft.AspNetCore.Mvc;
using OurBigDay.Api.Exceptions;

namespace OurBigDay.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult HandleException(BusinessException ex) => ex switch
    {
        NotFoundException => NotFound(ex.Message),
        ConflictException => Conflict(ex.Message),
        _ => BadRequest(ex.Message),
    };
}
