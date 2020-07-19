using System;
using System.Reflection;

using Aliencube.AzureFunctions.Extensions.OpenApi.Abstractions;
using Aliencube.AzureFunctions.Extensions.OpenApi.Configurations;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;
using Aliencube.AzureFunctions.Extensions.OpenApi.Extensions;
using Aliencube.AzureFunctions.Extensions.OpenApi.Resolvers;

using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

namespace Aliencube.AzureFunctions.Extensions.OpenApi
{
    /// <summary>
    /// This represents the context entity for <see cref="OpenApiHttpTrigger"/>.
    /// </summary>
    public class OpenApiHttpTriggerContext : IOpenApiHttpTriggerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiHttpTrigger"/> class.
        /// </summary>
        public OpenApiHttpTriggerContext()
        {
            var host = HostJsonResolver.Resolve();

            this.OpenApiInfo = OpenApiInfoResolver.Resolve(host);
            this.HttpSettings = host.GetHttpSettings();

            var filter = new RouteConstraintFilter();
            var helper = new DocumentHelper(filter);

            this.Document = new Document(helper);
            this.SwaggerUI = new SwaggerUI();
        }

        /// <inheritdoc />
        public virtual OpenApiInfo OpenApiInfo { get; }

        /// <inheritdoc />
        public virtual HttpSettings HttpSettings { get; }

        /// <inheritdoc />
        public virtual IDocument Document { get; }

        /// <inheritdoc />
        public virtual ISwaggerUI SwaggerUI { get; }

        /// <inheritdoc />
        public virtual NamingStrategy NamingStrategy { get; } = new CamelCaseNamingStrategy();

        /// <inheritdoc />
        public virtual Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        /// <inheritdoc />
        public virtual OpenApiSpecVersion GetOpenApiSpecVersion(string version = "v2")
        {
            var parsed = Enum.TryParse(version, true, out OpenApiVersionType output)
                             ? output
                             : throw new InvalidOperationException("Invalid Open API version");

            return this.GetOpenApiSpecVersion(parsed);
        }

        /// <inheritdoc />
        public virtual OpenApiSpecVersion GetOpenApiSpecVersion(OpenApiVersionType version = OpenApiVersionType.V2)
        {
            return version.ToOpenApiSpecVersion();
        }

        /// <inheritdoc />
        public virtual OpenApiFormat GetOpenApiFormat(string format = "json")
        {
            if (format.Equals("yml", StringComparison.InvariantCultureIgnoreCase))
            {
                format = "yaml";
            }

            var parsed = Enum.TryParse(format, true, out OpenApiFormatType output)
                             ? output
                             : throw new InvalidOperationException("Invalid Open API format");

            return this.GetOpenApiFormat(parsed);
        }

        /// <inheritdoc />
        public virtual OpenApiFormat GetOpenApiFormat(OpenApiFormatType format = OpenApiFormatType.Json)
        {
            return format.ToOpenApiFormat();
        }

        /// <inheritdoc />
        public virtual string GetSwaggerAuthKey(string key = "OpenApi__ApiKey")
        {
            var value = Environment.GetEnvironmentVariable(key);

            return value;
        }
    }
}
