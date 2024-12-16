using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace microservices.shared.Extensions
{
    public static class EndpointResultExt
    {
        public static IResult ToGenericResult<T>(this ServiceResult<T> result)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.OK => Results.Ok(result),
                HttpStatusCode.Created => Results.Created(result.UrlAsCreated, result),
                HttpStatusCode.BadRequest => Results.Problem(result.Fail!),
                HttpStatusCode.NotFound => Results.NotFound(result.Fail!),
                _ => Results.Problem(result.Fail!)
            };
        }

        public static IResult ToGenericResult(this ServiceResult result)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NoContent => Results.NoContent(),
                HttpStatusCode.NotFound => Results.NotFound(result.Fail!),
                _ => Results.Problem(result.Fail!)
            };
        }
    }
}
