University related project.
Simple Pascal like language compiler.


=============================================


Alphabet
------

 Letter = 'a'..'z' | 'A'..'Z' | '_'.
 Digit = '0'..'9'.
 Space = #9 | #10 | #13 | ' '.
 AnyChar = ' '..#255.

Lexems
-------

  Number = Digit {Digit}.
  Ident = Letter {Letter | Digit}.
  Delimiter = Space
  Keyword = 'OR' | 'DIV' | 'MOD' | 'AND' | 'NOT' |
            'READ' | 'WRITE'.
  SpecialSymbol = ':=' | 
                  '(' | ')' | ';' | '+' | '-' | '*'.
  OtherSymbol = ...

Grammar
---------

[1]  Program = {Operator ';'}.
[2]  Operator = AssignOp | ReadOp | WriteOp.
[3]  AssignOp = Ident ':=' Expression.
[4]  ReadOp = 'READ' Ident.
[5]  WriteOp = 'WRITE' Expression.
[6]  Expression = ['+' | '-'] Term {('+' | '-' | 'OR') Term}.
[7]  Term = Factor {('*' | 'DIV' | 'MOD' | 'AND') Factor}.
[8]  Factor = 'NOT' Factor | Number | Ident | '(' Expression ')'.


Examples
-------

Read A;
B := A*2;
Write B;


Read A;
Read B;
Write A+B;

Read A;
Read B;
C := (A*A+B*B)*2;
Write C;

