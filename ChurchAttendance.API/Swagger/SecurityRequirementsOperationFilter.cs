
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChurchAttendance.API.Swagger;

public sealed class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var allowsAnonymous =
            context.MethodInfo.IsDefined(
                typeof(AllowAnonymousAttribute), true)
            ||
            (context.MethodInfo.DeclaringType?.IsDefined(
                typeof(AllowAnonymousAttribute), true) ?? false);

        if (allowsAnonymous)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(
                    "Bearer",
                    null)] = new List<string>()
            });
    }
}
