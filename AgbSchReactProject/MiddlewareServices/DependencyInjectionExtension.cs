//using ApplicationCore.Repository;
using ApplicationCore.Services;
using ApplicationCore.Services.Interface;
using ApplicationCore.Services.Repository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AgbSchReactProject.MiddlewareServices
{
    public static class DependencyInjectionExtension
    {
        public static void AddScopedConfig(this IServiceCollection services)
        {

            // Registering the DB Context
            //services.AddScoped<DbContext, ApplicationDbContext>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddCors(opt =>
            {
                opt.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            // services.AddPdfHtml();
            //Registering repositories


        }
        public static void AddDocSwagger(this IServiceCollection service)
        {
            //service.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Membership Portal API", Version = "v1" });

            //});
            service.AddSwaggerGen(swagger =>
            {

                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

            });
        }
    }
    
}
