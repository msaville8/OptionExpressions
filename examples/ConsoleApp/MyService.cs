using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class MyService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly MyServiceOptions options;

        public MyService(ILogger<MyService> logger, IOptions<MyServiceOptions> options)
        {
            this.logger = logger;
            this.options = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation($"Service Name: {this.options.ServiceName}");
            this.logger.LogInformation($"Enabled: {this.options.Enabled}");
            this.logger.LogInformation($"Value: {this.options.Value}");
            return Task.CompletedTask;
        }
    }
}