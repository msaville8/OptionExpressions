using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace OptionExpressions
{
    internal class ConfigureNamedOptionsWithExpressions<TOptions> : IConfigureNamedOptions<TOptions>
        where TOptions : class
    {
        private readonly ExpressionOptions expressionOptions;
        private readonly IConfiguration configuration;

        public ConfigureNamedOptionsWithExpressions(ExpressionOptions expressionOptions, IConfiguration configuration)
        {
            this.expressionOptions = expressionOptions;
            this.configuration = configuration;
        }

        public void Configure(TOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        public void Configure(string name, TOptions options)
        {
            IConfiguration config = string.IsNullOrEmpty(name) ? this.configuration : this.configuration.GetSection(name);

            var binder = new ExpressionConfigurationBinder(this.expressionOptions);
            binder.BindInstance(options, config);
        }
    }
}