using System.Web.Http;
using WebActivatorEx;
using Jimismith.Versioning.Example;
using Swashbuckle.Application;
using Microsoft.Web.Http;
using Swashbuckle.Swagger;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Jimismith.Versioning.Example
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.MultipleApiVersions(
                            (apiDesc, version) =>
                            {
                                // get the versions specified by the controller
                                var controllerVersions = apiDesc.GetControllerAndActionAttributes<ApiVersionAttribute>()
                                    .SelectMany(attr => attr.Versions); ;
                                // get the versions specified by the action. these take precedence
                                var actionVersions = apiDesc.GetControllerAndActionAttributes<MapToApiVersionAttribute>()
                                    .SelectMany(attr => attr.Versions);

                                version = version.Replace("_", ".");

                                // if there are any action versions that match the current swagger version, use them
                                if (actionVersions.Any())
                                {
                                    return actionVersions.Any(v => $"v{v.ToString()}" == version);
                                }

                                // else use any controller versions that match
                                return controllerVersions.Any(v => $"v{v.ToString()}" == version);
                            },
                            (vc) =>
                            {
                                vc.Version("v1_1", "Versioned Api v1.1");
                                vc.Version("v1_0", "Versioned Api v1.0");
                            });
                        c.DocumentFilter<VersionHeaderDocumentFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.EnableDiscoveryUrlSelector();
                    });
        }
    }
}
