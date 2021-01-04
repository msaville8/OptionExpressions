parser grammar ExpressionParser;

options {
	tokenVocab=ExpressionLexer;
}

expression
	: booleanLiteral
	| integerLiteral
	| stringLiteral
	| functionCall
	| variableReference
	| expression op expression
	| PARENTHESIS_OPEN expression PARENTHESIS_CLOSE
	;

booleanLiteral
	: TRUE
	| FALSE
	;

integerLiteral
	: INTEGER_LITERAL
	;

stringLiteral
	: STRING_LITERAL
	;

functionCall
	: IDENTIFIER PARENTHESIS_OPEN expressionList PARENTHESIS_CLOSE
	;

expressionList
	: (expression (COMMA expression)* )?
	;

op
	: booleanOp
	| arithmeticOp
	;

booleanOp
	: LOGICAL_AND
	| LOGICAL_OR
	| EQUALS
	| NOT_EQUALS
	| LESS_THAN_OR_EQUALS
	| GREATER_THAN_OR_EQUALS
	| LESS_THAN
	| GREATER_THAN
	;

arithmeticOp
	: PLUS
	| HYPHEN
	| ASTERISK
	| FORWARD_SLASH
	| PERCENT
	;

variableReference
	: IDENTIFIER
	;