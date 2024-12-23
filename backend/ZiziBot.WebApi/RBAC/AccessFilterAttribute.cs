using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.RBAC;

[AttributeUsage(AttributeTargets.Method)]
public class AccessFilterAttribute : TypeFilterAttribute
{
    public AccessFilterAttribute(
        string flag,
        RoleLevel apiRoleLevel = RoleLevel.User,
        bool checkHeader = true,
        bool needAuthenticated = false
    ) : base(typeof(AccessFilterAuthorizationFilter))
    {
        Arguments = [
            apiRoleLevel,
            checkHeader,
            needAuthenticated
        ];
    }
}