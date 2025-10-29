using Microsoft.AspNetCore.Mvc;
using Store.G02.Domain.Contracts;
using Store.G02.persistence;
using Store.G02.Services;
using Store.G02.Shared.ErrorsModels;
using Store.G02.Web.Middlewares;

namespace Store.G02.Web.Extensions
{
    public static class Extensions
    {
        #region  Before Build
        public static IServiceCollection RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerServices();  // Swagger
            services.AddInfrastructureServices(configuration);  // DataBase
            services.AddApplicationServices(configuration); // Services

            services.ConfigureServices(); // Api Config

            return services;

        }
        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        private static IServiceCollection ConfigureServices(this IServiceCollection services)
        {

            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(m => m.Value.Errors.Any())
                                 .Select(m => new ValidationError()
                                 {
                                     Field = m.Key,
                                     Errors = m.Value.Errors.Select(errors => errors.ErrorMessage)
                                 });
                    var response = new ValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });


            return services;
        }
        #endregion


        #region After Build
        public static async Task<WebApplication> ConfigureMiddlewares(this WebApplication app)
        {

            await app.IntializeDatabaseAsync();

            app.UseGlobalErrorHandling();

            app.UseStaticFiles();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            return app;
        }

        private static async Task<WebApplication> IntializeDatabaseAsync(this WebApplication app)
        {
            #region Initialize Db
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // ASK CLR To Create Object from IDbIntializer
            await dbInitializer.InitializeAsync();
            #endregion

            return app;
        }
        private static WebApplication UseGlobalErrorHandling(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return app;
        }
        #endregion


    }
}
