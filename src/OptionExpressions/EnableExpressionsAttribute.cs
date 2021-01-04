using System;

namespace OptionExpressions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class EnableExpressionsAttribute : Attribute
    {
        public EnableExpressionsAttribute() { }

        public EnableExpressionsAttribute(bool enableFunctions = true, bool enableVariables = true)
        {
            EnableFunctions = enableFunctions;
            EnableVariables = enableVariables;
        }

        public bool? EnableFunctions { get; private set; }

        public bool? EnableVariables { get; private set; }

        internal ExpressionOptions ApplyTo(ExpressionOptions options)
        {
            return options with
            {
                EnableFunctions = EnableFunctions ?? options.EnableFunctions,
                EnableVariables = EnableVariables ?? options.EnableVariables
            };
        }
    }
}