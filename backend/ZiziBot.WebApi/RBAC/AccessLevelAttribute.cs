using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.RBAC;

[AttributeUsage(AttributeTargets.Method)]
public class AccessLevelAttribute : TypeFilterAttribute
{

    public AccessLevelAttribute(ApiRoleLevel apiRoleLevel) : base(typeof(AccessLevelAuthorizationFilter))
    {
        Arguments = new object[]
        {
            apiRoleLevel
        };
    }
}