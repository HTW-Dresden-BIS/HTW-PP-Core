using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using OclAspectTest;
using OCL;

namespace OclAspectTest
{
    public class OclTestProvider
    {
        private IEnumerable<Assembly> _targetAssemblies;
        private static bool _debugger = false;
        private static bool _customMethod = false;

        public static void AddConstraints(IEnumerable<string> targetAssemblies, string ocls, bool debugger, bool customMethod)
        {
            _debugger = debugger;
            _customMethod = customMethod;
            new OclTestProvider(targetAssemblies.Select(Assembly.Load), ocls);
        }
        private void CompileOCLs(string ocls)
        {
            var aspects = OclParser.ScanString(ocls);

            Console.WriteLine();

            var gens = new List<CodeGenerator>();
            foreach (Aspect aspect in aspects)
            {
                Console.WriteLine("Generating assembly for " + aspect.ConstraintName + ".");
                aspect.Print();
                gens.Add(GenCode(aspect));
            }

            Console.WriteLine();
            Console.WriteLine("Invoking Apply() methods.");
            foreach (var gen in gens)
            {
                gen.InvokeApplyMethod();
            }

            Console.WriteLine();
        }

        private OclTestProvider(IEnumerable<Assembly> targetAssemblies, string ocls)
        {
            _targetAssemblies = targetAssemblies;
            CompileOCLs(ocls);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private CodeGenerator GenCode(Aspect aspect)
        {
            // Console.WriteLine("Aspect: " + aspect.ToString());

            var gen = new CodeGenerator(
                _targetAssemblies,
                aspect,
                _debugger,
                _customMethod
            );
            // gen.InvokeApplyMethod();
            return gen;
        }
    }
}