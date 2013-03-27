using System;
using System.Text;
using System.IO;

namespace SimplePCourseProject
{
	public class Compiler
	{
		public static void Compile (string file, string assemblyName) {

			TextReader reader = new StreamReader(file);
			Scanner scanner = new Scanner(reader);
			Emitter emitter = new Emitter(assemblyName);
			Parser parser = new Parser(scanner, emitter, new DefaultDiagnostics());

			bool isProgram = parser.Parse();

			if (isProgram) {
				emitter.WriteExecutable();
			}
		}
	}
}

