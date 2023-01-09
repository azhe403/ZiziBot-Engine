using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ZiziBot.WebApi.Providers;

public class ActionHidingConvention : IActionModelConvention
{

	public void Apply(ActionModel action)
	{
		if (action.ActionName.Contains("session"))
		{
			action.ApiExplorer.IsVisible = false;
		}
	}
}