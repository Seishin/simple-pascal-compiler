using System;
using System.IO;

namespace SimplePCourseProject
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			if (args.Length == 0) {
				Console.WriteLine("Syntax: scsc {/r:filename} <source file> [<result exe file>]");
				return -1;
			}
			int i = 0;

			string assemblyName;
			if (args.Length == i+2) assemblyName = args[i+1];
			else assemblyName = Path.ChangeExtension(args[i], "exe");
					
			Compiler.Compile(args[i], assemblyName);

			return 0;
		}
	}
}
