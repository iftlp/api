using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Extensions.Collections;
using System.Net;

namespace Shizima.WebAPI.Extentions.Controller
{
    public static class PaginationResult
    {
        /// <summary>
        /// Generate header response with the desired paginated list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="controller"></param>
        /// <param name="entities">A paginated list of entities object</param>
        /// <returns></returns>
        public static IEnumerable<T> PaginatedResult<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            if (entities.TotalRecords == 0)
            {
                controller.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            else if (entities.HasNextPage)
            {
                controller.Response.StatusCode = StatusCodes.Status206PartialContent;
            }
            else
            {
                controller.Response.StatusCode = StatusCodes.Status200OK;
            }
            
            controller.GeneratePaginatedHeader(entities);
            return entities.AsEnumerable();
        }

        /// <summary>
        /// Set response with the desire status and return the result object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="statusCode">Returned HTTP status code</param>
        /// <param name="result">The object return in the body</param>
        /// <returns></returns>
        public static T ResponseWithStatus<T>(this ControllerBase controller, HttpStatusCode statusCode, T result = default)
        {
            controller.Response.StatusCode = (int)statusCode;
            return result;
        }

        private static void GeneratePaginatedHeader<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            controller.Response.Headers.Append("x-pagination-index", entities.StartIndex.ToString());
            controller.Response.Headers.Append("x-pagination-size", (entities.EndIndex-entities.StartIndex+1).ToString());
            controller.Response.Headers.Append("x-pagination-total", entities.TotalRecords.ToString());
        }
    }
}
