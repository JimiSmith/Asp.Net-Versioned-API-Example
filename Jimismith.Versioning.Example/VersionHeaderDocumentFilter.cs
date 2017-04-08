using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http.Description;
using Microsoft.Web.Http;

namespace Jimismith.Versioning.Example
{
    public class VersionHeaderDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            // get the version that this swagger doc represents
            var currentVersion = swaggerDoc.info.version.Replace("v", "").Replace("_", ".");
            // iterate through all the paths in the swagger doc
            foreach (var path in swaggerDoc.paths.Values)
            {
                // for every action on that path, add the version to the media type
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.get);
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.post);
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.head);
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.options);
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.put);
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.delete);
                UpdateOperationVersionInfo(apiExplorer, currentVersion, path.patch);
            }
        }

        private static void UpdateOperationVersionInfo(IApiExplorer apiExplorer,
            string currentVersion,
            Operation operation)
        {
            if (operation != null)
            {
                var currentVersionMediaType = $"v={currentVersion}";
                var apiDesc = apiExplorer.ApiDescriptions
                    .FirstOrDefault(a => operation.operationId.StartsWith($"{a.ActionDescriptor.ControllerDescriptor.ControllerName}_{a.ActionDescriptor.ActionName}"));
                var version = apiDesc?.GetControllerAndActionAttributes<ApiVersionAttribute>()
                    .FirstOrDefault(attr => attr.Versions.Select(v => v.ToString()).Contains(currentVersion));
                operation.deprecated = version?.Deprecated ?? false;
                operation.produces = operation.produces.Select(p => $"{p};{currentVersionMediaType}").ToList();
            }
        }
    }
}