using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.RBAC;

[AttributeUsage(AttributeTargets.Method)]
public class AccessFilterAttribute : TypeFilterAttribute
{
    public AccessFilterAttribute(
        string flag,
        RoleLevel roleLevel
    ) : base(typeof(AccessFilterAuthorizationFilter))
    {
        Arguments = [
            flag,
            roleLevel
        ];
    }
}