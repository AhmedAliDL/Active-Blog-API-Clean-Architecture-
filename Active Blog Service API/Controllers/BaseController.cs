using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Active_Blog_Service_API.Controllers
{
    [Route("api")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly ISender _mediator;
        public BaseController(ISender mediator)
        {
            _mediator = mediator;
        }
    }
}
