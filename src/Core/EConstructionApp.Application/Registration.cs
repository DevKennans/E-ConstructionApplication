using EConstructionApp.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;

namespace EConstructionApp.Application
{
    public static class Registration
    {
        public static void AddApplication(this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            services.AddValidatorsFromAssembly(assembly);
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("eng");

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
        }
    }
}
