using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using WebApi;
using WebApi.Shared;

namespace SAPWebAPI.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/{Controller}")]
    public class CompanyController : ControllerBase
    {
        private readonly IRfcContext _rfcContext;


        public CompanyController(IRfcContext rfcContext)
        {
            _rfcContext = rfcContext;
        }

        [HttpGet]
        public Task<IActionResult> Get()
        {

            return _rfcContext.GetCompanies()
                    .ToActionResult();
        }
    }
}
