using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VirtualRoulette.Filters
{
    public sealed class BadRequestResult : ObjectResult
    {
        public BadRequestResult(string message) : base(message)
        {
            StatusCode = 400;
        }

        public BadRequestResult(ModelStateDictionary message) : base(GetMessages(message))
        {
            StatusCode = 400;
        }

        private static IEnumerable<object> GetMessages(ModelStateDictionary modelState)
            => modelState.Keys.SelectMany(key =>
                modelState[key].Errors.Select(x => new
                {
                    Key = key,
                    ErrorMessage = x.ErrorMessage
                }));
    }
}
