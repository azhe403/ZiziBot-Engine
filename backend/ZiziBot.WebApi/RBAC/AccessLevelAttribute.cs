using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.RBAC;

[AttributeUsage(AttributeTargets.Method)]
public class AccessLevelAttribute : TypeFilterAttribute
{

    public AccessLevelAttribute(AccessLevelEnum accessLevelEnum) : base(typeof(AccessLevelAuthorizationFilter))
    {
        Arguments = new object[]
        {
            accessLevelEnum
        };
    }
}