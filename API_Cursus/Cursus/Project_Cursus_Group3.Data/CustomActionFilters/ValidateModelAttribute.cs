using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Project_Cursus_Group3.Data.Model.LoginModel;
using Project_Cursus_Group3.Data.Model.UserModel;

namespace Project_Cursus_Group3.Data.CustomActionFilters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult();
                return;
            }

            // Validate RegisterLoginModel
            if (context.ActionArguments.TryGetValue("registerDTO", out var registerObj) && registerObj is RegisterLoginModel registerModel)
            {
                ValidateRegisterLoginModel(registerModel, context);
            }

            // Get RoleId from Claims
            var roleIdClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "RoleId");
            int roleId = roleIdClaim != null ? int.Parse(roleIdClaim.Value) : 0;

            // Validate UserProfileUpdateModel
            if (context.ActionArguments.TryGetValue("userProfileUpdateModel", out var updateProfileObj) && updateProfileObj is UserProfileUpdateModel updateProfileModel)
            {
                ValidateUserProfileUpdateModel(updateProfileModel, roleId, context);
            }
        }

        private void ValidateRegisterLoginModel(RegisterLoginModel model, ActionExecutingContext context)
        {
            var today = DateTime.Today;

            // Validate DOB
            if (model.DOB == default)
            {
                context.ModelState.AddModelError("DOB", "Date of birth is required.");
            }
            else
            {
                var age = today.Year - model.DOB.Year;
                if (model.DOB > today)
                {
                    context.ModelState.AddModelError("DOB", "Date of birth cannot be in the future.");
                }
                else if (age < 12 || (age == 12 && model.DOB > today.AddYears(-12)))
                {
                    context.ModelState.AddModelError("DOB", "You must be at least 12 years old.");
                }
            }

            // Validate Certification
            if (model.RoleId == 2 && model.Certification == null)
            {
                context.ModelState.AddModelError("Certification", "Certification is required for instructor.");
            }
            else if (model.RoleId != 2 && model.Certification != null)
            {
                context.ModelState.AddModelError("Certification", "Only Instructor can provide a certification.");
            }

            // Check if errors
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        private void ValidateUserProfileUpdateModel(UserProfileUpdateModel model, int roleId, ActionExecutingContext context)
        {
            var today = DateTime.Today;

            // Validate DOB
            if (model.DOB == default)
            {
                context.ModelState.AddModelError("DOB", "Date of birth is required.");
            }
            else
            {
                var age = today.Year - model.DOB.Year;
                if (model.DOB > today)
                {
                    context.ModelState.AddModelError("DOB", "Date of birth cannot be in the future.");
                }
                else if (age < 12 || (age == 12 && model.DOB > today.AddYears(-12)))
                {
                    context.ModelState.AddModelError("DOB", "You must be at least 12 years old.");
                }
            }

            // Validate Certification
            //if (roleId == 2 && model.Certification == null)
            //{
            //    context.ModelState.AddModelError("Certification", "Certification is required for instructor.");
            //}
            if (roleId != 2 && model.Certification != null)
            {
                context.ModelState.AddModelError("Certification", "Only Instructor can provide a certification.");
            }

            // Check if errors
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
