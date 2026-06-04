using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Presentation.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        public RedisCacheAttribute()
        {
            
        }
        public override Task OnActionExecutionAsync
            (
               ActionExecutingContext context,
               ActionExecutionDelegate next
            )
        {
           
        }
    }
}
