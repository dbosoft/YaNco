using System.Net;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace WebApi
{
    public static class ApiResultExtensions
    {
        public static Task<IActionResult> ToActionResult<T>(this EitherAsync<RfcErrorInfo, T> result)
        {
            return result.Match(
                r => (IActionResult)new ObjectResult(r),
                l => new ObjectResult(l.Message){StatusCode = (int) HttpStatusCode.InternalServerError});


        }
    }
}