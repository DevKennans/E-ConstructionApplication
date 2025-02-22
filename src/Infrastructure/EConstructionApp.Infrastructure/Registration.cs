using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EConstructionApp.Infrastructure
{
    public static class Registration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
