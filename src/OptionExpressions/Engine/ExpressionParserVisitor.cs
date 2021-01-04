using Antlr4.Runtime.Misc;
using System;
using System.Globalization;
using System.Linq;
using OptionExpressions.Engine.Generated;

namespace OptionExpressions.Engine
{
    internal sealed class ExpressionParserVisitor : ExpressionParserBaseVisitor<ExpressionSyntaxNode>
    {
        public override ExpressionSyntaxNode VisitExpression([NotNull] ExpressionParser.ExpressionContext context)
        {
            if (context.functionCall() is not null and var functionCall)
            {
                return VisitFunctionCall(functionCall);
            }

            if (context.variableReference() is not null and var variableRef)
            {
                return VisitVariableReference(variableRef);
            }

            if (context.booleanLiteral() is not null and var booleanLiteral)
            {
                return VisitBooleanLiteral(booleanLiteral);
            }

            if (context.integerLiteral() is not null and var integerLiteral)
            {
                return VisitIntegerLiteral(integerLiteral);
            }

            if (context.stringLiteral() is not null and var stringLiteral)
            {
                return VisitStringLiteral(stringLiteral);
            }

            if (context.expression() is not null and var expressions)
            {
                if (expressions.Length == 1)
                {
                    return VisitExpression(expressions[0]);
                }
                else if (expressions.Length > 1 && context.op() is not null and var binaryOp)
                {
                    if (binaryOp.booleanOp() is not null and var booleanOp)
                    {
                        return new BooleanExpression
                        {
                            Left = VisitExpression(expressions[0]) as Expression,
                            Right = VisitExpression(expressions[1]) as Expression,
                            Operator = ParseBooleanOperator(booleanOp)
                        };
                    }
                    else if (binaryOp.arithmeticOp() is not null and var arithmeticOp)
                    {
                        return new ArithmeticExpression
                        {
                            Left = VisitExpression(expressions[0]) as Expression,
                            Right = VisitExpression(expressions[1]) as Expression,
                            Operator = ParseArithmeticOperator(arithmeticOp)
                        };
                    }
                }
            }

            throw new NotImplementedException();
        }

        public override ExpressionSyntaxNode VisitBooleanLiteral([NotNull] ExpressionParser.BooleanLiteralContext context)
        {
            return new BooleanLiteral
            {
                Value = context.TRUE() is not null
            };
        }

        public override ExpressionSyntaxNode VisitIntegerLiteral([NotNull] ExpressionParser.IntegerLiteralContext context)
        {
            return new IntegerLiteral
            {
                Value = int.Parse(context.INTEGER_LITERAL().GetText(), NumberStyles.Integer, CultureInfo.InvariantCulture)
            };
        }

        public override ExpressionSyntaxNode VisitStringLiteral([NotNull] ExpressionParser.StringLiteralContext context)
        {
            string text = context.GetText();
            return new StringLiteral
            {
                Value = text.Substring(1, text.Length - 2)
            };
        }

        public override ExpressionSyntaxNode VisitFunctionCall([NotNull] ExpressionParser.FunctionCallContext context)
        {
            return new FunctionCall
            {
                FunctionName = context.IDENTIFIER().GetText(),
                Arguments = VisitExpressionList(context.expressionList()) as ExpressionList
            };
        }

        public override ExpressionSyntaxNode VisitExpressionList([NotNull] ExpressionParser.ExpressionListContext context)
        {
            return new ExpressionList
            {
                Expressions = context.expression().Select(expr => VisitExpression(expr) as Expression).ToList()
            };
        }

        public BooleanOperator ParseBooleanOperator(ExpressionParser.BooleanOpContext context)
        {
            BooleanOperator op;

            if (context.LOGICAL_AND() is not null)
            {
                op = BooleanOperator.LogicalAnd;
            }
            else if (context.LOGICAL_OR() is not null)
            {
                op = BooleanOperator.LogicalOr;
            }
            else if (context.EQUALS() is not null)
            {
                op = BooleanOperator.Equals;
            }
            else if (context.NOT_EQUALS() is not null)
            {
                op = BooleanOperator.NotEquals;
            }
            else if (context.LESS_THAN_OR_EQUALS() is not null)
            {
                op = BooleanOperator.LessThanOrEquals;
            }
            else if (context.GREATER_THAN_OR_EQUALS() is not null)
            {
                op = BooleanOperator.GreaterThanOrEquals;
            }
            else if (context.LESS_THAN() is not null)
            {
                op = BooleanOperator.LessThan;
            }
            else if (context.GREATER_THAN() is not null)
            {
                op = BooleanOperator.GreaterThan;
            }
            else
            {
                throw new NotImplementedException();
            }

            return op;
        }

        public ArithmeticOperator ParseArithmeticOperator(ExpressionParser.ArithmeticOpContext context)
        {
            ArithmeticOperator op;

            if (context.PLUS() is not null)
            {
                op = ArithmeticOperator.Addition;
            }
            else if (context.HYPHEN() is not null)
            {
                op = ArithmeticOperator.Subtraction;
            }
            else if (context.ASTERISK() is not null)
            {
                op = ArithmeticOperator.Multiplication;
            }
            else if (context.FORWARD_SLASH() is not null)
            {
                op = ArithmeticOperator.Division;
            }
            else
            {
                throw new NotImplementedException();
            }

            return op;
        }

        public override ExpressionSyntaxNode VisitVariableReference([NotNull] ExpressionParser.VariableReferenceContext context)
        {
            return new VariableReference
            {
                VariableName = context.GetText()
            };
        }
    }
}