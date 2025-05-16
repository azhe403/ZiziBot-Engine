using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZiziBot.WebApi.Providers;

public class HidePathDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.Keys.ToList();
        var filteredPaths = paths.Where(p => p.Contains("session") || p.Contains("webhook")).ToList();

        filteredPaths.ForEach(path => swaggerDoc.Paths.Remove(path));
        swaggerDoc.Paths.Remove("/");
    }
}