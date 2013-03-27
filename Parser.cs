using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SimplePCourseProject
{
	public class Parser
	{
		private Scanner scanner;
		private Emitter emit;
		private DefaultDiagnostics diagnostics;

		private Token token;
		private Token previousToken;

		// List that holds all of the declared variables.
		private List<LocalVariable> localVariables = new List<LocalVariable>();

		public Parser (Scanner scanner, Emitter emittrer, DefaultDiagnostics diagnostics)
		{
			this.scanner = scanner;
			this.emit = emittrer;
			this.diagnostics = diagnostics;
		}

		// Returns the next token from the scanner.
		public void ReadNextToken ()
		{
			token = scanner.Next();
		}

		// The entry method of the parser.
		public bool Parse ()
		{
			ReadNextToken();

			return IsProgram() && token is EOFToken;
		}

		#region CheckMethods
		// Checking if the token is a keyword token and is defined as a keyword. 
		// If True - returns true and loads the next token, if False - returns false.
		public bool CheckKeyword (string keyword) 
		{
			bool result = (token is KeywordToken) && ((KeywordToken) token).value.Equals(keyword);

			if (result) 
			{
				// Preserves the previous token 
				previousToken = token;
				ReadNextToken();
			}

			return result;
		}

		// Checking if the token is an ident.
		// If True - returns true and loads the next token, if False - returns false.
		public bool CheckIdent ()
		{
			bool result = (token is IdentToken);

			if (result) 
			{
				// Preserves the previous token 
				previousToken = token;
				ReadNextToken();
			}

			return result;
		}

		// Checking if the token is a special symbol token and is defined as such.
		// If True - returns true and loads the next token, if False - returns false.
		public bool CheckSpecialSymbol (string symbol)
		{
			bool result = (token is SpecialSymbolToken) && ((SpecialSymbolToken) token).value.Equals(symbol);

			if (result) 
			{
				// Preserves the previous token 
				previousToken = token;
				ReadNextToken();
			}

			return result;
		}

		// Cheking if the token is a number.
		// If True - returns true and loads the next token, if False - returns false.
		public bool CheckNumber ()
		{
			bool result = (token is IntegerToken);

			if (result) 
			{
				// Preserves the previous token 
				previousToken = token;
				ReadNextToken();
			}

			return result;
		}

		#endregion CheckMethods

		// Skips all the tokens until it finds special symbol ";"
		public void SkipUntilSemiColon ()
		{
			Token tok;

			do 
			{
				tok = scanner.Next();
			} while (!((tok is EOFToken) || (tok is SpecialSymbolToken) && ((tok as SpecialSymbolToken).value == ";")));
		}

		#region DiagnosticMethods

		// Error Messeges
		public void Error (string message) 
		{
			diagnostics.Error(token.line, token.column, message);
			SkipUntilSemiColon();
		}

		public void Error (string message, Token token) 
		{
			diagnostics.Error(token.line, token.column, message);
			SkipUntilSemiColon();
		}

		public void Error (string message, Token token, params object[] parms) 
		{
			diagnostics.Error(token.line, token.column, string.Format (message, parms));
			SkipUntilSemiColon();
		}

		// Warning Messeges
		public void Warning(string message)
		{
			diagnostics.Warning(token.line, token.column, message);
		}
		
		public void Warning(string message, Token token)
		{
			diagnostics.Warning(token.line, token.column, message);
		}
		
		public void Warning(string message, Token token, params object[] par)
		{
			diagnostics.Warning(token.line, token.column, string.Format(message, par));
		}
		
		public void Note(string message)
		{
			diagnostics.Note(token.line, token.column, message);
		}

		// Note Messeges
		public void Note(string message, Token token)
		{
			diagnostics.Note(token.line, token.column, message);
		}
		
		public void Note(string message, Token token, params object[] par)
		{
			diagnostics.Note(token.line, token.column, string.Format(message, par));
		}

		#endregion DiagnosticMethods

		// Checks if it's a valid program.
        public bool IsProgram()
        {
            while (IsOperator())
            {
				// Checks for the special symbol ';'
                if (!CheckSpecialSymbol(";"))
                {
                    Error("Expects: a special symbol ';'!");
                    return false;
                }

                if(token is EOFToken)
                    break;
            }
            return true;
            

        }	

		// Checks if it's an operator or identifier.
        public bool IsOperator ()
		{
			if (token is KeywordToken && ((KeywordToken)token).value.Equals ("READ")) {
				EvaluateReadOp ();
				return true;
			} else if (token is KeywordToken && ((KeywordToken)token).value.Equals ("WRITE")) {
				EvaluateWriteOp ();
				return true;
			} else if (token is IdentToken) {
				EvaluateIdent ();
				return true;
			} 

            return false;
        }

		// Evaluating the READ operator.
		public bool EvaluateReadOp ()
		{

			if (!CheckKeyword ("READ")) {
				Error ("Expects: keyword 'READ'");
				return false;
			}

			if (!CheckIdent ()) {
				Error("Exprects: an identificator!");
				return false;
			}

			IdentToken variable = previousToken as IdentToken;

			// Adds a new variable into the variable's list and emits it.
			AddNewVariable(variable.value);
			emit.ReadIntValue(localVariables.Find(i => i.name == variable.value).varInfo, variable.value);

            return true;
        }

		// Evaluate the WRITE operator.
        public bool EvaluateWriteOp ()
		{
			if (!CheckKeyword ("WRITE")) {
				Error ("Expects: keyword WRITE!");
				return false;
			}

			if (!IsExpression ()) {
				return false;
			}

			// Printing the result.
			emit.PrintResult();

            return true;
        }

		// Evaluate the identifier
		public bool EvaluateIdent ()
		{
			IdentToken variable = token as IdentToken;

			if (!CheckIdent ()) {
				Error ("Expects: an identifier!");
				return false;
			} else {
				// If the identifier isn't in the variable's list, adds it as a new variable.
				if (localVariables.Find (i => i.name == variable.value) == null) {
					AddNewVariable (variable.value);
				}
			} 

			if (!CheckSpecialSymbol (":=")) {
				Error ("Expects: an assign operator ':='!");
				return false;
			}

			if (!IsExpression ()) {
                return false;
			}

			// Store the result into the variable's location.
			emit.StoreToVariable(localVariables.Find(i => i.name == variable.value).varInfo);

			return true;
		}

		// Cheking if it's a factor.
        public bool IsFactor()
        {
            Token literal = token;

			// If it finds Keyword token "NOT" emits the NOT instruction.
            if (CheckKeyword("NOT") == true)
            {
                if (IsFactor() && !(token is EOFToken))
                {
                    emit.AddNotOp();
                    return true;
                }
                else
                {
                    Error("Expects: an identificator!");
                    return false;
                }
                
            }
			// Checks if it's a number and emits an number
            else if (CheckNumber() == true)
            {
				IntegerToken number = literal as IntegerToken;
				emit.AddIntValue(number.value);
                return true;
            }
			// Checks if it's an ident and loads the variable
            else if (CheckIdent() == true)
            {
			    IdentToken name = literal as IdentToken;

				// If it's not located in the variable's list, fires an error that the 
				// variable isn't declared.
				if(localVariables.Find(i => i.name == name.value) == null) {
					Error("The variable: " + name.value + " isn't declared!");
					return false;
				} else {
					emit.LoadLocalVar(localVariables.Find(i => i.name == name.value).varInfo);
				}

                return true;
            }
			// Checks for special symbol "(".
            else if (CheckSpecialSymbol("(") == true)
            {
				// Checks if next is an expression.
                if (IsExpression())
                    if (CheckSpecialSymbol(")") == true)
                        return true;
                    else
                    {
                        Error("Expects: a special symbol ')'");
                        return false;
                    }
                else
                {
                    Error("Expects: an expression!");
                    return false;
                }
            }
          
            return false;
        }
	
		// Checks if it's a term.
        public bool IsTerm()
        {
            if(IsFactor())
            {
                if (token is EOFToken)
                    return true;

				// Looping through the tokens to find the tokens for the Multiplicative Operations. 
                while (CheckKeyword("DIV") || CheckKeyword("MOD") || CheckKeyword("AND") || CheckSpecialSymbol("*"))
                {
					Token op = previousToken as Token;

					// If it's not a factor returns
                    if (!IsFactor())
						return false;

					// Emits the relevant operation
					if(op is KeywordToken && ((KeywordToken)op).value.Equals("DIV")) {
						emit.AddMultiplicativeOperator("DIV");
					} else if(op is KeywordToken && ((KeywordToken)op).value.Equals("MOD")) {
						emit.AddMultiplicativeOperator("MOD");
					} else if(op is KeywordToken && ((KeywordToken)op).value.Equals("AND")) {
						emit.AddMultiplicativeOperator("AND");
					} else if(op is SpecialSymbolToken && ((SpecialSymbolToken)op).value.Equals("*")) {
						emit.AddMultiplicativeOperator("*");
					} 
				}
                    
                return true;
            }
            
			return false;
        }

		// Checks if it's an expression
        public bool IsExpression()
        {
            if(IsTerm() == true)
            {
				// Loops through the token until it finds the tokens for the relevant Additive Operations.
                while(CheckKeyword("OR") || CheckSpecialSymbol("+") || CheckSpecialSymbol("-"))
                {
					// Loads previous token which is the relevant operation
					Token op = previousToken as Token;

                    if (!IsTerm())
                    {
                        Error("Exprects: a term!");
                        return false;
                    }

					// Emits the relevant operations
					if(op is SpecialSymbolToken && ((SpecialSymbolToken)op).value.Equals("+")) {
						emit.AddAdditiveOperator("+");
					} else if(op is SpecialSymbolToken && ((SpecialSymbolToken)op).value.Equals("-")) {
						emit.AddAdditiveOperator("-");
					} else if(op is KeywordToken && ((KeywordToken)op).value.Equals("OR")) {
						emit.AddAdditiveOperator("OR");
					}
                }

                return true;
            }
            return false;
        }

		// Methods that add a new variable into the variable's list
		private void AddNewVariable (string name)
		{
			LocalVariable newLocalVariable = new LocalVariable ();
			newLocalVariable.name = name;
			newLocalVariable.varInfo = emit.AddLocalVariable ();

			localVariables.Add(newLocalVariable);
		}

		// Variable's model class
		private class LocalVariable 
		{
			public string name;
			public LocalBuilder varInfo;
		}
	}
}