using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace ECommerceSystem.Api.SwaggerConfig // Hoặc một namespace phù hợp với project của bạn
{
    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Kiểm tra xem endpoint có được đánh dấu [AllowAnonymous] không
            var allowAnonymous = context.ApiDescription.CustomAttributes().OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return; // Nếu có AllowAnonymous, bỏ qua việc yêu cầu xác thực
            }

            // Kiểm tra xem endpoint có yêu cầu xác thực (ví dụ: có [Authorize]) không
            var hasAuthorize =
                context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (!hasAuthorize)
            {
                return; // Nếu không có [Authorize], bỏ qua
            }

            // Áp dụng token cho tất cả endpoint, không kiểm tra [Authorize]
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                }
            };
        }
    }
}