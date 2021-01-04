using Antlr4.Runtime;
using System;
using System.Globalization;
using System.Linq;
using OptionExpressions.Engine.Generated;

namespace OptionExpressions.Engine
{
    internal sealed class ExpressionEngine
    {
        private readonly ExpressionOptions options;

        private ExpressionEngine(ExpressionOptions options)
        {
            this.options = options;
        }

        public static string Evaluate(string expression, ExpressionOptions options)
        {
            var inputStream = new AntlrInputStream(expression);
            var tokenSource = new ExpressionLexer(inputStream);
            var tokenStream = new CommonTokenStream(tokenSource);
            var parser = new ExpressionParser(tokenStream) { BuildParseTree = true };
            var visitor = new ExpressionParserVisitor();

            var expressionRoot = visitor.VisitExpression(parser.expression()) as Expression;

            var engine = new ExpressionEngine(options);
            return engine.Execute(expressionRoot);
        }

        private string Execute(Expression expression)
        {
            var result = expression switch
            {
                FunctionCall functionCall => ExecuteFunction(functionCall),
                VariableReference variableRef => ResolveVariable(variableRef.VariableName),
                StringLiteral stringLiteral => stringLiteral.Value,
                IntegerLiteral integerLiteral => integerLiteral.Value.ToString(CultureInfo.InvariantCulture),
                BooleanLiteral booleanLiteral => booleanLiteral.Value.ToString(),
                // TODO: BooleanExpression and ArithmeticExpression
                _ => throw new NotImplementedException()
            };

            return result.ToString();
        }

        private string ExecuteFunction(FunctionCall functionCall)
        {
            if (!this.options.EnableFunctions)
            {
                throw new InvalidOperationException("Functions are disabled.");
            }

            if (!this.options.Functions.TryGetValue(functionCall.FunctionName, out var callback))
            {
                throw new NotImplementedException($"Function '{functionCall.FunctionName}' is undefined.");
            }

            var evaluatedArgs = functionCall.Arguments.Expressions.Select(Execute);

            return callback.Invoke(evaluatedArgs.ToArray());
        }

        private string ResolveVariable(string variableName)
        {
            if (!this.options.Variables.TryGetValue(variableName, out string value))
            {
                throw new InvalidOperationException($"Variable '{variableName}' is undefined.");
            }

            return value;
        }
    }
}