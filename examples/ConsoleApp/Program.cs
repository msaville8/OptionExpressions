using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptionExpressions;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<MyService>();
                })
                .UseOptionExpressions(configure =>
                {
                    configure.RegisterFunction("getCustomValue", GetCustomValue);
                })
                .Build();

            await host.StartAsync();
        }

        // Custom function handlers must accept a string array as the parameter
        private static bool GetCustomValue(string[] args)
        {
            return args.Length > 0;
        }
    }
}