lexer grammar ExpressionLexer;

TRUE: 'true';
FALSE: 'false';

POINT: '.';
PARENTHESIS_OPEN: '(';
PARENTHESIS_CLOSE: ')';
COMMA: ',';
PLUS: '+';
HYPHEN: '-';
ASTERISK: '*';
FORWARD_SLASH: '/';
PERCENT: '%';
LESS_THAN: '<';
GREATER_THAN: '>';
LESS_THAN_OR_EQUALS: '<=';
GREATER_THAN_OR_EQUALS: '>=';
LOGICAL_AND: '&&';
LOGICAL_OR: '||';
EQUALS: '==';
NOT_EQUALS: '!=';
APOSTROPHE: '\'';

INTEGER_LITERAL: (PLUS | HYPHEN)? [0-9]+;
STRING_LITERAL: '\'' (~('\n' | '\r'))* '\'';

IDENTIFIER: LetterCharacter (LetterCharacter | DigitCharacter)*;

fragment LetterCharacter: [A-Z] | [a-z];
fragment DigitCharacter: [0-9];

WHITESPACE: [ \n\t\r]+ -> skip;