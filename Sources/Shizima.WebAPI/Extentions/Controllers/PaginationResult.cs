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
        /// <param name="controller"></param>
        /// <param name="entities">A paginated list of entities object</param>
        /// <returns></returns>
        public static IEnumerable<T> PaginatedResult<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            if (entities.TotalRecords == 0)
            {
                controller.Response.StatusCode = StatusCodes.Status204NoContent;
                controller.GeneratePaginatedHeaderTotal(entities);
            }
            else if (entities.HasNextPage)
            {
                controller.Response.StatusCode = StatusCodes.Status206PartialContent;
                controller.GeneratePaginatedHeader(entities);
            }
            else
            {
                controller.Response.StatusCode = StatusCodes.Status200OK;
                controller.GeneratePaginatedHeader(entities);
            }
            
            return entities.AsEnumerable();
        }
        
        private static void GeneratePaginatedHeader<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            controller.GeneratePaginatedHeaderIndex(entities);
            controller.GeneratePaginatedHeaderSize(entities);
            controller.GeneratePaginatedHeaderTotal(entities);
        }
        
        private static void GeneratePaginatedHeaderIndex<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            controller.Response.Headers.Append("x-pagination-index", entities.StartIndex.ToString());
        }
        
        private static void GeneratePaginatedHeaderSize<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            controller.Response.Headers.Append("x-pagination-size", (entities.EndIndex-entities.StartIndex+1).ToString());
        }
        
        private static void GeneratePaginatedHeaderTotal<T>(this ControllerBase controller, PaginatedList<T> entities)
        {
            controller.Response.Headers.Append("x-pagination-total", entities.TotalRecords.ToString());
        }
    }
}
