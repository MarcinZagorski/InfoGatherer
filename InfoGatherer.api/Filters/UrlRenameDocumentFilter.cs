using InfoGatherer.api.Helpers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace InfoGatherer.api.Filters
{
    public class UrlRenameDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();

            foreach (var path in swaggerDoc.Paths)
            {
                var segments = path.Key.Trim('/').Split('/');

                var transformedSegments = segments.Select(segment =>
                    segment.StartsWith("{") && segment.EndsWith("}")
                        ? segment
                        : segment.ToCamelCase());

                var newPath = "/" + string.Join("/", transformedSegments);

                paths[newPath] = path.Value;
            }

            swaggerDoc.Paths = paths; 
        }
    }
}
