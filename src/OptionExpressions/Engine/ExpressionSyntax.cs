using System.Collections.Generic;

namespace OptionExpressions.Engine
{
    internal abstract record ExpressionSyntaxNode;

    internal abstract record Expression : ExpressionSyntaxNode;

    internal record ExpressionList : ExpressionSyntaxNode
    {
        public List<Expression> Expressions { get; set; }
    }

    internal record LiteralValue : Expression;

    internal record StringLiteral : LiteralValue
    {
        public string Value { get; set; }
    }

    internal record IntegerLiteral : LiteralValue
    {
        public long Value { get; set; }
    }

    internal record BooleanLiteral : LiteralValue
    {
        public bool Value { get; set; }
    }

    internal record FunctionCall : Expression
    {
        public string FunctionName { get; set; }
        public ExpressionList Arguments { get; set; }
    }

    internal abstract record BinaryExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    internal record BooleanExpression : BinaryExpression
    {
        public BooleanOperator Operator { get; set; }
    }

    internal enum BooleanOperator
    {
        LogicalAnd,
        LogicalOr,
        Equals,
        NotEquals,
        LessThanOrEquals,
        GreaterThanOrEquals,
        LessThan,
        GreaterThan
    }

    internal record ArithmeticExpression : BinaryExpression
    {
        public ArithmeticOperator Operator { get; set; }
    }

    internal enum ArithmeticOperator
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Modulus
    }

    internal record VariableReference : Expression
    {
        public string VariableName { get; set; }
    }
}