using System;
using System.IO;
using System.Text;

namespace SimplePCourseProject
{
	public class Scanner
	{
		// Defining the language lexems and special symbols
		private static readonly string keywords = " OR DIV MOD AND NOT READ WRITE ";
		private static readonly string specialSymbols = "();+-*:";
		private static readonly string specialSymbolsPairs = " := ";

		private const char EOF = '\u001a';
		private const char CR = '\r'; 
		private const char LF = '\n';
		private const char ESCAPE = '\\'; 

		// Text reader that reads from the file
		private TextReader reader;

		// Single char symbol that will be readed 
		private char ch;

		// Variables that holds the line's and column's numbers
		private int line, column;

		public Scanner (TextReader r) 
		{
			this.reader = r;
			this.line = 1;
			this.column = 0;

			ReadNextChar();
		}

		// Method that reads the next char
		private void ReadNextChar ()
		{
			int ch1 = reader.Read ();
			column++;

			ch = (ch1 < 0) ? EOF : (char)ch1;

			if (ch == CR) {
				line++;
				column = 0;
			} else if (ch == LF) {
				column = 0;
			}
		}

		// Method that unescapes the escaped symbols or 
		// returns the same char if it's not a special symbol 
		private char Unescape (char ch)
		{
			switch (ch) {
			case 'n' : return '\n';
			case 't' : return '\t';
			case '\'' : return '\'';
			case '0' : return '\0';
			case '"' : return '\"';
			default : return ch;
			}
		}

		// Method that reads and returns a token
		public Token Next ()
		{
			int start_column;
			int start_line;

			StringBuilder str;

			while (true) 
			{
				start_column = column;
				start_line = line;

				if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_') 
				{
					str = new StringBuilder();

					while (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '_')
					{
						str.Append(ch);
						ReadNextChar();
					}

					string id = str.ToString();

					if (keywords.Contains(" " + id + " "))
					{
						return new KeywordToken(start_line, start_column, id);
					}

					return new IdentToken(start_line, start_column, id);
				} 
				else if (ch >= '0' && ch <= '9')
				{
					str = new StringBuilder();

					while(ch >= '0' && ch <= '9')
					{
						str.Append(ch);
						ReadNextChar();
					}

					string id = str.ToString();

					return new IntegerToken(start_line, start_column, Convert.ToInt32(id));
				} 
				else if (ch == '"')
				{
					str = new StringBuilder();

					ReadNextChar();

					while (ch != '"' && ch != EOF) 
					{
						str.Append(ch);
						ReadNextChar();
					}

					ReadNextChar();

					string id = str.ToString();

					return new StringToken(start_line, start_column, id);
				}
				else if (specialSymbols.Contains(ch.ToString()))
				{
					char ch1 = ch;
					ReadNextChar();
					char ch2 = ch;

					string id;

					if (specialSymbolsPairs.Contains(" " + ch1 + ch2 + " ")) 
					{
						ReadNextChar();

						id = ch1.ToString() + ch2.ToString();
						return new SpecialSymbolToken(start_line, start_column, id);
					}

					id = ch1.ToString();

					return new SpecialSymbolToken(start_line, start_column, id);
				} 
				else if (ch == ' ' || ch == '\t' || ch == CR || ch == LF)
				{
					ReadNextChar();
					continue;
				}
				else if (ch == EOF)
				{
					return new EOFToken(start_line, start_column);
				} 
				else 
				{
					string id = ch.ToString();
					ReadNextChar();

					return new OtherToken(start_line, start_column, id);
				}
			}
		}
	}
}

