using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZiziBot.WebApi.Providers;

public class PathPrefixSwaggerDocumentFilter : IDocumentFilter
{
	private readonly string _prefix;

	public PathPrefixSwaggerDocumentFilter(string prefix)
	{
		_prefix = prefix;
	}

	public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
	{
		var paths = swaggerDoc.Paths.Keys.ToList();

		paths.ForEach(
			path => {
				var pathToChange = swaggerDoc.Paths[path];
				swaggerDoc.Paths.Remove(path);
				swaggerDoc.Paths.Add(_prefix + path, pathToChange);
			}
		);
	}
}