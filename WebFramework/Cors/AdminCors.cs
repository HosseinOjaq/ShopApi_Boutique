using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace WebFramework.Cors
{
    public static class AdminCors
    {
        public static void AddCustomeAdminCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowCors",
                    builder =>
                    {
                        builder.AllowAnyOrigin().WithMethods(
                            HttpMethod.Get.Method,
                            HttpMethod.Put.Method,
                            HttpMethod.Post.Method,
                            HttpMethod.Delete.Method).AllowAnyHeader().WithExposedHeaders("CustomHeader")
                            .WithOrigins("https://localhost:5001", "https://localhost:44361");
                    });
            });
        }
    }
}
