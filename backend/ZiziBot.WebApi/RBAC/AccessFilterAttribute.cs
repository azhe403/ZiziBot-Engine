using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.RBAC;

[AttributeUsage(AttributeTargets.Method)]
public class AccessFilterAttribute : TypeFilterAttribute
{
    public AccessFilterAttribute(RoleLevel apiRoleLevel = RoleLevel.User, bool checkHeader = true) : base(typeof(AccessFilterAuthorizationFilter))
    {
        Arguments = new object[] {
            apiRoleLevel,
            checkHeader
        };
    }
}