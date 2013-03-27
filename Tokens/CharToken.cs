using System;
using System.Text;

namespace SimplePCourseProject
{
	public class CharToken : Token
	{
		public char value;

		public CharToken (int line, int column, char value) : base (line, column)
		{
			this.value = value;
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			s.AppendFormat("line {0}, column {1}: {2} - {3}", line, column, value, GetType());
			return s.ToString();
		}
	}
}

