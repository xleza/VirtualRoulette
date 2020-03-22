using Microsoft.AspNetCore.Mvc.Filters;

namespace VirtualRoulette.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult(context.ModelState);
            }

            base.OnActionExecuting(context);
        }
    }
}
