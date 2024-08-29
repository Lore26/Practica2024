using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TrainsAPI.Swagger;

public class AuthorizationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorizeAttribute = context.ApiDescription.ActionDescriptor
            .EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .Any();

        if (!hasAuthorizeAttribute)
        {
            return;
        }

        var securityScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        };

        operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
    }
}
