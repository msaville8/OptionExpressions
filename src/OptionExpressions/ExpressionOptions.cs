using System;
using System.Collections.Generic;

namespace OptionExpressions
{
    public record ExpressionOptions
    {
        public string Prefix { get; set; } = "[";

        public string Suffix { get; set; } = "]";

        public bool EnableFunctions { get; set; } = true;

        public bool EnableVariables { get; set; } = true;

        internal Dictionary<string, Func<string[], string>> Functions { get; set; }
            = new Dictionary<string, Func<string[], string>>();

        internal Dictionary<string, string> Variables { get; set; }
            = new Dictionary<string, string>();

        public void RegisterFunction<TResult>(string functionName, Func<string[], TResult> callback)
        {
            Functions[functionName] = (context) => callback(context)?.ToString();
        }

        public void RegisterVariable(string variableName, string value)
        {
            Variables[variableName] = value;
        }
    }
}