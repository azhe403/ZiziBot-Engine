using Microsoft.AspNetCore.Mvc.Filters;

namespace ZiziBot.Application.RBAC;

public class AccessFilter : Attribute, IAsyncAuthorizationFilter
{
	private readonly AccessLevel _accessLevel;

	public AccessFilter(AccessLevel accessLevel)
	{
		_accessLevel = accessLevel;

	}

	public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
	{
		throw new NotImplementedException();
	}
}