using System;

namespace SimplePCourseProject
{
	public abstract class Diagnostics
	{
		public abstract void Error (int line, int column, string message);
		public abstract void Warning (int line, int column, string message);
		public abstract void Note (int line, int column, string message);
		public abstract int GetErrorCount();
		public abstract void BeginSourceFile(string sourceFile);
		public abstract void EndSourceFile();
	}
}

