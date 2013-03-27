using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace SimplePCourseProject
{
	public class Emitter
	{
		// Assembly's configuration fields
		private AssemblyBuilder asmBuilder;
		private ModuleBuilder moduleBuilder;
		private TypeBuilder typeBuilder;
		private MethodBuilder methodBuilder; 
		private ILGenerator ilGen;

		private string executableName;
		private string dir;
		private string moduleName;

		public Emitter (string name)
		{
			this.executableName = name;

			AppDomain appDomain = AppDomain.CurrentDomain;
			AssemblyName asmName = new AssemblyName();
			asmName.Name = Path.GetFileNameWithoutExtension(name);

			this.dir = Path.GetDirectoryName(name);
			this.moduleName = Path.GetFileName(name);

			this.asmBuilder = appDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, dir);

			this.moduleBuilder = asmBuilder.DefineDynamicModule(asmName + "Module", moduleName);

			this.typeBuilder = moduleBuilder.DefineType("type", TypeAttributes.Public);

			this.methodBuilder = typeBuilder.DefineMethod("main", MethodAttributes.Public | MethodAttributes.Static, null, null);

			ilGen = methodBuilder.GetILGenerator();

		}

		// Writting the executable file.
		public void WriteExecutable() 
		{
			ilGen.Emit(OpCodes.Ret);

			this.asmBuilder.SetEntryPoint(methodBuilder);
			this.typeBuilder.CreateType();
			this.asmBuilder.Save(Path.GetFileName(executableName));
		}

		// Adding a new local variable of type Int32
		public LocalBuilder AddLocalVariable ()
		{
			LocalBuilder localVariable = ilGen.DeclareLocal (typeof(Int32));

			return localVariable;
		}

		// Loading an already stored local variable.
		public void LoadLocalVar (LocalBuilder varInfo)
		{
			ilGen.Emit(OpCodes.Ldloc, varInfo);
		}

		// Assigning a value to a variable.
		public void StoreToVariable (LocalBuilder variable)
		{
			ilGen.Emit(OpCodes.Stloc, variable);
		}

		// Adding an int value. 
		public void AddIntValue (long value)
		{
			// If it's between Min and Max value of Int32 emits 4bit constant.
			// If't higher emits 8bit constant.
			if (value >= Int32.MinValue && value <= Int32.MaxValue) {
				ilGen.Emit (OpCodes.Ldc_I4, (Int32)value);	
			} else {
				ilGen.Emit(OpCodes.Ldc_I8, value);	
			}
		}

		// Reading an integer value from the console.
		public void ReadIntValue (LocalBuilder variable, string variableName)
		{
			MethodInfo writeMethod = typeof(Console).GetMethod("Write", new Type[] { typeof(string) });

			MethodInfo readLineMethod = typeof(Console).GetMethod("ReadLine", new Type[] { }); 
			MethodInfo parseMethod = typeof(Int32).GetMethod("Parse", new Type[] { typeof(string) });

			// Showing a hint with the variable name.
			ilGen.Emit(OpCodes.Ldstr, variableName + ": ");
			ilGen.Emit(OpCodes.Call, writeMethod);

			// Reading and storing the variable in it's location.
			ilGen.Emit(OpCodes.Call, readLineMethod);
			ilGen.Emit(OpCodes.Call, parseMethod);
			ilGen.Emit(OpCodes.Stloc, variable);
		}

		// Printing the result.
		public void PrintResult ()
		{
			MethodInfo writeMethod = typeof(Console).GetMethod("Write", new Type[] { typeof(string) });
			ilGen.Emit(OpCodes.Ldstr, "Result: ");
			ilGen.Emit(OpCodes.Call, writeMethod);

			MethodInfo writeLineMethod = typeof(Console).GetMethod("Write", new Type[] { typeof(Int32) });
			ilGen.Emit(OpCodes.Call, writeLineMethod);

			ilGen.Emit(OpCodes.Ldstr, "\n\n");
			ilGen.Emit(OpCodes.Call, writeMethod);
		}

		// Emitting a NOT operator.
		public void AddNotOp ()
		{
			ilGen.Emit(OpCodes.Not);
		}

		// Emitting an Additive Operator.
		public void AddAdditiveOperator (string op)
		{
			switch (op) {
			case "+":
				ilGen.Emit(OpCodes.Add);
				break;
			case "-":
				ilGen.Emit(OpCodes.Sub);
				break;
			case "OR":
				ilGen.Emit(OpCodes.Or);
				break;
			}
		}

		// Emitting a Multiplicative Operator.
		public void AddMultiplicativeOperator (string op)
		{
			switch (op) {
			case "MOD":
				ilGen.Emit(OpCodes.Rem);
				break;
			case "DIV":
				ilGen.Emit(OpCodes.Div);
				break;
			case "AND":
				ilGen.Emit(OpCodes.And);
				break;
			case "*":
				ilGen.Emit(OpCodes.Mul);
				break;
			}
		}
	}
}

