using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using TrainsAPI.DTOs;

namespace TrainsAPI.Utilities;

public static class SwaggerExtensions
{
    public static TBuilder AddRoutesFilterParameters<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithOpenApi(options =>
        {
            AddPaginationParameters(options);

            options.Parameters.Add(new OpenApiParameter
            {
                Name = "StartStationId",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "integer"
                }
            });

            options.Parameters.Add(new OpenApiParameter
            {
                Name = "EndStationId",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "integer"
                }
            });

            options.Parameters.Add(new OpenApiParameter
            {
                Name = "departureTime",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                }
            });

            options.Parameters.Add(new OpenApiParameter
            {
                Name = "trainTypeId",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "integer"
                }
            });

            options.Parameters.Add(new OpenApiParameter
            {
                Name = "IncludePast",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "boolean"
                }
            });

            return options;
        });
    }

    public static TBuilder AddPaginationParameters<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithOpenApi(options =>
        {
            AddPaginationParameters(options);
            return options;
        });
    }

    private static void AddPaginationParameters(OpenApiOperation openApiOperation)
    {
        openApiOperation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
        {
            Name = "Page",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Schema = new Microsoft.OpenApi.Models.OpenApiSchema
            {
                Type = "integer",
                Default = new OpenApiInteger(PaginationDTO.PageInitialValue)
            }
        });

        openApiOperation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
        {
            Name = "RecordsPerPage",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Schema = new Microsoft.OpenApi.Models.OpenApiSchema
            {
                Type = "integer",
                Default = new OpenApiInteger(PaginationDTO.RecordsPerPageInitialValue)
            }
        });
    }
}
