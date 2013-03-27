using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace SimplePCourseProject
{
	public class Table
	{

		private Stack<Dictionary<string, TableSymbol>> symbolTable;


		public Table ()
		{
			this.symbolTable = new Stack<Dictionary<string, TableSymbol>>();
		}

		public override string ToString ()
		{
			StringBuilder str = new StringBuilder ();

			int i = symbolTable.Count;

			str.AppendFormat ("=======\n");

			foreach (var table in symbolTable)
			{
				str.AppendFormat("---[{0}]---\n", i--);

				foreach (var row in table) 
				{
					str.AppendFormat ("[{0}] {1}\n", row.Key, row.Value);
				}
			}

			str.AppendFormat ("========\n");

			return str.ToString();
		}

		public TableSymbol Add (TableSymbol symbol) 
		{
			symbolTable.Peek().Add(symbol.value, symbol);

			return symbol;
		}

		public TableSymbol AddLocalVar (IdentToken token, LocalBuilder localBuilder)
		{
			LocalVarSymbol result = new LocalVarSymbol(token, localBuilder);
			symbolTable.Peek().Add(token.value, result);

			return result;
		}

		public TableSymbol GetSymbol (string ident) 
		{
			TableSymbol result;

			foreach (Dictionary<string, TableSymbol> table in symbolTable) 
			{
				if (table.TryGetValue (ident, out result)) 
				{
					return result;
				}
			}

			return null;
		}
	}
}

