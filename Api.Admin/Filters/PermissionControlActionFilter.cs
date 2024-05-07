using Common;
using Data.Contracts;
using Entities.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace Api.Admin.Filters
{
    public class PermissionControlActionFilter : IAuthorizationFilter
    {
        private readonly IRolePermissionRepository rolePermissionRepository;
        public PermissionControlActionFilter(IRolePermissionRepository rolePermissionRepository)
        {
            this.rolePermissionRepository = rolePermissionRepository;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var actionMarkedToIgnorePermissionCheck =
                actionDescriptor.EndpointMetadata.OfType<IgnorePermissionCheckAttribute>().Any();

            var isAllowAnonymous =
            actionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (actionMarkedToIgnorePermissionCheck || isAllowAnonymous)
            {
                return;
            }

            if (!(context.HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity!.Claims?.Any() != true)
            {
                context.Result = new NotFoundObjectResult("درخواست نامعتبر");
                return;
            }

            var userId = claimsIdentity.GetUserId<int>();
            var actionFullName = "";
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                var actionName = descriptor.ActionName;
                var controllerName = descriptor.ControllerName;
                actionFullName = controllerName + actionName;
            }

            var hasPermissionResult = rolePermissionRepository.HasPermission(userId, actionFullName).Result;
            if (!hasPermissionResult)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}