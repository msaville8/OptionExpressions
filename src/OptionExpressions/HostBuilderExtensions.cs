using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

namespace OptionExpressions
{
    public static class OptionExpressionsHostBuilderExtensions
    {
        public static IHostBuilder UseOptionExpressions(this IHostBuilder builder, Action<ExpressionOptions> configure = null)
        {
            var options = new ExpressionOptions();

            AddDefaultVariables(options);
            AddDefaultFunctions(options);

            configure?.Invoke(options);

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(options);
                services.AddSingleton(typeof(IConfigureOptions<>), typeof(ConfigureNamedOptionsWithExpressions<>));
            });

            return builder;
        }

        private static void AddDefaultVariables(ExpressionOptions options)
        {
            options.RegisterVariable("assemblyName", Assembly.GetEntryAssembly().GetName().Name);
            options.RegisterVariable("assemblyVersion", Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        private static void AddDefaultFunctions(ExpressionOptions options)
        {
            options.RegisterFunction("concat", args => string.Concat(args));
            options.RegisterFunction("strlen", args => args.First().Length);
        }
    }
}