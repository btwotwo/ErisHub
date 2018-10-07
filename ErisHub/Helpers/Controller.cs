
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ErisHub.Helpers
{
    public class Controller : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public string CurrentUserId => User?.Claims.First(x => x.Type == "id")?.Value;

        public new BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
        {
            var errors = new Errors();

            foreach (var model in modelState)
            {
                foreach (var error in model.Value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return base.BadRequest(errors);
        }
        protected BadRequestObjectResult BadRequest(IEnumerable<IdentityError> identityErrors)
        {
            var errors = new Errors();

            foreach (var error in identityErrors)
            {
                errors.Add(error.Description);
            }

            return base.BadRequest(errors);
        }
        protected BadRequestObjectResult BadRequest(IdentityResult identityResult)
        {
            var errors = new Errors();

            foreach (var error in identityResult.Errors)
            {
                errors.Add(error.Description);
            }

            return base.BadRequest(errors);
        }
        protected BadRequestObjectResult BadRequest(string message)
        {
            var errors = new Errors();

            errors.Add(message);

            return base.BadRequest(errors);
        }
    }
}
