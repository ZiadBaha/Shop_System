#region All
//namespace Shop_System.Extentions
//{
//    public static class SwaggerServiceExtention
//    {
//        public static IServiceCollection AddSwaggerService(this IServiceCollection service)
//        {
//            // Add API Explorer services
//            service.AddEndpointsApiExplorer();

//            // Add Swagger generator
//            service.AddSwaggerGen();

//            // Return the updated service collection
//            return service;
//        }
//        public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
//        {
//            // Enable Swagger middleware
//            app.UseSwagger();

//            // Enable Swagger UI middleware
//            app.UseSwaggerUI();

//            // Return the updated application
//            return app;
//        }

//    }
//} 
#endregion

using Microsoft.OpenApi.Models;

namespace Shop_System.Extentions
{
    public static class SwaggerServiceExtention
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection service)
        {
            // Add API Explorer services
            service.AddEndpointsApiExplorer();

            // Add Swagger generator with token support
            service.AddSwaggerGen(options =>
            {
                // Define the security scheme for Bearer token
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token in the text input below. Example: Bearer abcdefghijklmnop"
                });

                // Add a global security requirement for all endpoints
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                });
            });

            // Return the updated service collection
            return service;
        }

        public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
        {
            // Enable Swagger middleware
            app.UseSwagger();

            // Enable Swagger UI middleware
            app.UseSwaggerUI();

            // Return the updated application
            return app;
        }
    }
}

